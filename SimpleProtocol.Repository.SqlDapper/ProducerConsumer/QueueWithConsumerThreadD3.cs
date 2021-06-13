//Kopie z D3Soft.Lib.Common.ProducerConsumer
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace SimpleProtocol.Repository.SqlDapper.ProducerConsumer
{
    public interface IConsumerThreadExceptionProcessing
    {
        void ExceptionOccured(Exception p_Exception);
    }

    /// <summary>
    /// Implementace vzoru Producer-Consumer (Zde pozadovano vice producer, jenom jeden consumer)
    /// Nemuzu pouzit .Net System.Collections.Concurrent.BlockingCollection&lt;T&gt;, protoze chci vyzvedavat Items ne dle poradi vlozeni (FIFO), ale dle sve logiky a rozliseni IQueueItemGroupId - do ktere group dany Item spada
    ///     - v pripade pouziti u Ares fronty jedno obsluzne vlakno a obsluhovat vice group metodou RotateQueue
    ///     - v pripade pouziti AuditWCF logovani davat do group dle ConnectString a cele groupy pokud mozno ukladat pod jednou sql transakci (vyber zaznamu metodou ByQueue)
    /// Trida, kdy muzu vkladat do fronty zaznamy z vice klientskych vlaken a zde se tvori jedno vlakno na pozadi a tyto zaznamy fronty zpracovava. 
    /// </summary>
    /// <typeparam name="T">Datova entita do fronty, ktera implementuje IQueueItemGroupId</typeparam>
    public abstract class QueueWithConsumerThreadD3<T> where T : class, IQueueItemGroupId
    {
        public IConsumerThreadExceptionProcessing ExceptionProcessing;
        /// <summary>
        /// Privatni pomocna trida zaobalujici ConcurrentQueue _Data , abych jednoduse mohl mit pristupk QueueGroupId a kontroloval, ze se mi tam nemichaji zaznamy z ruznych front
        /// </summary>
        private class QueueByGroupId
        {
            public DateTime LastModified { get; private set; }
            public string QueueGroupId { get; private set; }
            private readonly ConcurrentQueue<T> _Data = new ConcurrentQueue<T>();
            public void Enqueue(T p_Item)
            {
                if (_Data.Count == 0)
                {   //Vkladam prvni, uschovam si QueueGroupId
                    QueueGroupId = p_Item.QueueGroupId;
                }
                else if (p_Item.QueueGroupId != QueueGroupId)
                {   //pojistovaci kontrola, ze je vse ok
                    throw new Exception("Chyba algoritmu, do fronty group dle Id se dostal zaznam s jinym Id");
                }
                _Data.Enqueue(p_Item);
                LastModified = DateTime.Now;
            }
            public bool TryDequeue(out T p_Result)
            {
                return _Data.TryDequeue(out p_Result);
            }
            public int Count => _Data.Count;
        }
        
        #region public members

        public QueueConsumerFindModeItem ConsumerFindModeItem;
        /// <summary>
        /// Jestli ma zpracovavat davkove a ceka na urcity pocet polozek, nez vyvola udalost WorkItems(List p_Items)
        /// Default je 0 a nezpracovava davkove, tedy ihned po pridani zaznamu do fronty vyvola WorkItem(p_Item, int p_QueueGroupItemsCount)
        /// </summary>
        public int? WorkBatchItems;

        /// <summary>
        /// Jestli ma zpracovat davkove a cena urcity cas od prvniho vlozeneho zaznamu, nez vyvola udalost WorkItems(List p_Items)
        /// Nastaveni WorkBatchTimeout bez nastaven9 WorkBatchItems nemá význam a bude ignorováno. Pokud není nastaveno WotkBatchItems, nezpracovává se dávkově ale ihned po jednom záznamu přes metodu WorkItem
        /// </summary>
        public TimeSpan? WorkBatchTimeout;

        #endregion

        #region private members

        private readonly List<QueueByGroupId> _Queue;
        private readonly ReaderWriterLockSlim _QueueLock;
        private Thread _Thread;
        private readonly object _ThreadRunningLock = new object();

        #endregion

        #region ctor

        public QueueWithConsumerThreadD3()
        {
            _Queue = new List<QueueByGroupId>();
            _QueueLock = new ReaderWriterLockSlim();
        }

        #endregion

        #region public

        public void AddItems(List<T> p_Items)
        {
            if (p_Items != null)
            {
                p_Items.ForEach(i => AddItem(i));
            }
        }

        public void AddItem(T p_Item)
        {
            if (p_Item != null)
            {
                _QueueLock.EnterUpgradeableReadLock();
                try
                {
                    QueueByGroupId queueSameGroupId = _Queue.Find(q => q.QueueGroupId == p_Item.QueueGroupId);
                    if (queueSameGroupId != null)
                    {
                        //Debug.WriteLine("Queue.AddItem: " + p_Item.ToString());
                        queueSameGroupId.Enqueue(p_Item);
                    }
                    else
                    {
                        _QueueLock.EnterWriteLock();
                        try
                        {
                            //Debug.WriteLine("Queue.AddItem: " + p_Item.ToString());
                            QueueByGroupId queueByGroupId = new QueueByGroupId();
                            queueByGroupId.Enqueue(p_Item);
                            _Queue.Add(queueByGroupId);
                        }
                        finally
                        {
                            _QueueLock.ExitWriteLock();
                        }
                    }
                }
                finally
                {
                    _QueueLock.ExitUpgradeableReadLock();
                }
                CheckThreadWorking();
            }
        }

        public int QueueItemsCount
        {
            get
            {
                int result = 0;
                _QueueLock.EnterReadLock();
                try
                {
                    _Queue.ForEach(q => result += q.Count);
                }
                finally
                {
                    _QueueLock.ExitReadLock();
                }
                return result;
            }
        }

        #endregion

        #region public abstract virtual - musi implementovat potomek

        /// <summary>
        /// Tuto metodu vola vlakno zpracovani a zde si potomek napise implementaci, jak zaznam z fronty zpracovat. Prvky jsou vyzvedavany v poradi dle nastaveneho ProcessType
        /// </summary>
        /// <param name="p_Item"></param>
        /// <param name="p_QueueGroupItemsCount"></param>
        public abstract void WorkItem(T p_Item, int p_QueueGroupItemsCount);

        /// <summary>
        /// Pokud mam WorkBatchItems nebo WorkBatchTimeout, potom zpracovavam davkove a pripravena davka dat je zaslana do teto metody WorkItems
        /// Nedaval jsem abstract, aby stavajici implementace nepadly a nemusely upgradovat na novy interface. Proto jen virtual aby jen kdo chce si implementoval
        /// </summary>
        /// <param name="p_Item"></param>
        public virtual void WorkItems(List<T> p_Item)
        {
            throw new NotImplementedException();
        }

        public virtual void LogException(Exception e)
        {
            ExceptionProcessing?.ExceptionOccured(e);
            //potomek si napise kde ma logovat chyby z obsluzneho vlakna
        }

        #endregion

        #region private methods

        /// <summary>
        /// Po pridani do fronty se zkontroluje, ze se pusti obsluzne vlakno, ktere bude frontu zpracovavat
        /// </summary>
        private void CheckThreadWorking()
        {
            lock (_ThreadRunningLock)
            {   //musim pod zamkem.
                //TODO: Misto _Thread pres Task - bere vlakna z ThreadPoolu a je mene narocne na zdroje
                //Jinak chybne udela toto: jedno vlakno prijde pres priznak IsAlive=false, tam projde i druhe vlakno. To prvni zavola Start a to druhe potom chce taky zavolat Start a padne na ThreadStateException: Thread is running or terminated; it cannot restart.
                if (_Thread == null || !_Thread.IsAlive)
                {
                    _Thread = new Thread(ThreadWork);
                    //abyc uzavrenim hlavni aplikace ukoncil i toto vlakno, davam ho jako Background. Dopad: ztrati se pripadna obsluhovana fronta z pameti
                    _Thread.IsBackground = true;
                    _Thread.Start();
                }
            }
        }

        /// <summary>
        /// Metoda pustena vlaknem obsluhy fronty. Dohledava zaznamy ve fronte a kazdy zaznam zpracovava virtualni metodou WorkItem
        /// </summary>
        private void ThreadWork()
        {
            try
            {
                T item;
                int queueGroupItemsCount;
                if (!WorkBatchItems.HasValue)
                {   //nemam zpracovavat davkove, ale po jednom zaznamu
                    while ((item = GetNextItem(out queueGroupItemsCount)) != null)
                    {
                        //TODO:? Pokud spadne zpracovani jednoho zaznamu na vyjimku, tak konci cele vlakno a nezpracuje se dalsi zaznam, pokud jine vlakno neda neco do fronty a tim znovu spusti vlakno ke zpracovani. 
                        //Je toto dobre? Asi ne, proto davam i zde do try/catch
                        try
                        {
                            WorkItem(item, queueGroupItemsCount);
                        }
                        catch (Exception e)
                        {   //Dany zaznam se nepovedlo zpracovat, loguju a jdu na pripadny dalsi zaznam
                            LogException(e);
                        }
                    }
                }
                else
                {   //WorkbatchItems.HasValue - mam teda zpracovavat davkove
                    if (!WorkBatchTimeout.HasValue)
                    {
                        //throw new Exception("Pokud je nastavno WorkBatchItems, pak se musi nastavit i WorkBatchTimeout. Co kdyby mi zustalo mene zaznamu nez je velikost davky a to bych nikdy nezpracoval");
                        LogException(new Exception("Pokud je nastavno WorkBatchItems, pak se musi nastavit i WorkBatchTimeout. Co kdyby mi zustalo mene zaznamu nez je velikost davky a to bych nikdy nezpracoval"));
                        return;
                    }
                    DateTime started = DateTime.Now;
                    while (QueueItemsCount > 0)
                    {
                        List<T> items = GetNextItems(ref started);
                        if (items != null)
                        {
                            try
                            {
                                WorkItems(items);
                            }
                            catch (Exception e)
                            {   //Dany zaznam se nepovedlo zpracovat, loguju a jdu na pripadny dalsi zaznam
                                LogException(e);
                            }                        
                        }
                        else
                        {   //mam nejake data ale neni to na celou davku, budu kontrolovat timeout - at nedelam extra casto a cpu usetrim, tak na chvili uspim
                            Thread.Sleep(100);
                        }
                    }
                }
            }
            catch (Exception e)
            {   //jede ve svem vlakne tento kod - tak odchytavam chyby a muze me potomek naimplementovat pres virtual LogException jak ma logovat
                LogException(e);
            }
        }

        private int _QueueGroupActualReadingIndex;  //default 0
        private bool _QueueGroupActualReadingIndexNextIncrement;    //default false                
        /// <summary>
        /// Metoda, ktera vraci polozky z fronty dle sveho algoritmu
        /// Prochazi jednotlive QueueGroupItem a z kazde vemze vzdy jeden QueueItem a pristi vyzvednuti GetNextItem bude brat polozku z dalsiho QueueGroupItem
        /// </summary>
        /// <returns>Nalezeny Item ve fronte nebo NULL pokud uz je fronta prazdna</returns>
        private T GetNextItem(out int p_QueueGroupItemsCount)
        {
            T result = null;
            p_QueueGroupItemsCount = 0;

            _QueueLock.EnterUpgradeableReadLock();
            try
            {
                while (result == null && _Queue.Count > 0)
                {   //mohl jsem v dane group dojit k tomu, ze je prazdna, tu group vyhodim, result = null a musim jit na dalsi group
                    //najdi index v listu, ktery budes zpracovavat. Po kazdem precteni se musim posunout na dalsi
                    if (_QueueGroupActualReadingIndexNextIncrement)
                    {
                        _QueueGroupActualReadingIndex = (_QueueGroupActualReadingIndex + 1 >= _Queue.Count || _QueueGroupActualReadingIndex < 0) ? 0 : ++_QueueGroupActualReadingIndex;
                    }
                    QueueByGroupId group = _Queue.ElementAt(_QueueGroupActualReadingIndex);
                    //Debug.WriteLine(String.Format("Fronta ma {0} groups. Aktualni group {1} ma {2} zaznamy", _Items.Count, _QueueGroupActualReadingIndex, group.Count));
                    if (group.TryDequeue(out result))
                    {
                        if (ConsumerFindModeItem == QueueConsumerFindModeItem.RotateQueue)
                        {   //pokud mam rotovat fronty, tak ted jsem neco nasel a proto rotuju na dalsi frontu
                            _QueueGroupActualReadingIndexNextIncrement = true;
                        }
                        p_QueueGroupItemsCount = group.Count;
                    }
                    else
                    {   //v dane group uz nic neni, tak ji vyhazuju
                        _QueueLock.EnterWriteLock();
                        try
                        {
                            _Queue.RemoveAt(_QueueGroupActualReadingIndex);
                        }
                        finally
                        {
                            _QueueLock.ExitWriteLock();
                        }
                        //Debug.WriteLine(String.Format("Vyhodil jsem group {0}, uz je zpracovana", _QueueGroupActualReadingIndex));
                        _QueueGroupActualReadingIndexNextIncrement = false;
                    }
                }
            }
            finally
            {
                _QueueLock.ExitUpgradeableReadLock();
            }
            return result;
        }

        private List<T> GetNextItems()
        {
            if (ConsumerFindModeItem != QueueConsumerFindModeItem.ByQueue)
            {   //tento algoritmus je jenom pro cteni z jedne fronty
                throw new NotImplementedException();
            }

            List<T> result = null;
            int i = -1;
            while(true)
            {   //zkus prochazet od pocatku vsechny fronty - zda najdes nejakou s daty nebo timeoutem
                QueueByGroupId group;
                _QueueLock.EnterReadLock();
                try
                {
                    i++;
                    if (i >= _Queue.Count)
                    {   //uz jsem se indexem dostal nekde mimo a tolik front uz neexistuje
                        return null;
                    }
                    group = _Queue.ElementAt(i);
                }
                finally
                {
                    _QueueLock.ExitReadLock();
                }
                if (group.Count > 0 && (group.Count >= WorkBatchItems || DateTime.Now - group.LastModified > WorkBatchTimeout))
                {   //z teto fronty muzes zpracovat data, ber je
                    result = new List<T>();
                    while (result.Count < WorkBatchItems && group.Count > 0)
                    {
                        T item;
                        if (group.TryDequeue(out item))
                        {
                            result.Add(item);
                        }
                    }
                    return result;
                }
                if (group.Count == 0)
                {   //v dane group uz nic neni, tak ji vyhazuju
                    _QueueLock.EnterWriteLock();
                    try
                    {
                        _Queue.RemoveAt(_QueueGroupActualReadingIndex);
                    }
                    finally
                    {
                        _QueueLock.ExitWriteLock();
                    }
                    //Debug.WriteLine(String.Format("Vyhodil jsem group {0}, uz je zpracovana", _QueueGroupActualReadingIndex));
                    _QueueGroupActualReadingIndexNextIncrement = false;
                }
            }
        }

        private List<T> GetNextItems(ref DateTime p_LastModified)
        {
            List<T> items = new List<T>();
            if (ConsumerFindModeItem == QueueConsumerFindModeItem.RotateQueue)
            {   //nepotrebuju data celistve z jedne fronty, beru cele data odkudkoliv z front
                bool davkuZpracujPlna = QueueItemsCount >= WorkBatchItems.Value;
                bool davkuZpracujTimeout = WorkBatchTimeout.HasValue ? DateTime.Now - p_LastModified >= WorkBatchTimeout : false;
                if (davkuZpracujPlna || davkuZpracujTimeout)
                {
                    T item;
                    int queueGroupItemsCount;
                    while ((items.Count < WorkBatchItems.Value) && (item = GetNextItem(out queueGroupItemsCount)) != null)
                    {
                        items.Add(item);
                    }
                    //zpracoval nejakou davku, zaznacim ze odted cekam na dalsi naplneni nove davky
                    p_LastModified = DateTime.Now;
                    return items;
                }
            }
            else if (ConsumerFindModeItem == QueueConsumerFindModeItem.ByQueue)
            {   //kdyz hledas data, tak jedine davku celou z jedne fronty
                return GetNextItems();
            }
            return null;
        }

        #endregion
    }
}
