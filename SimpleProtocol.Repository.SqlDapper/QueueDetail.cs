using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using Dapper.Contrib.Extensions;
using SimpleProtocol.Repository.SqlDapper.ProducerConsumer;

namespace SimpleProtocol.Repository.SqlDapper
{
    public class QueueDetail : QueueWithConsumerThreadD3<QueueItemDetail>
    {
        public override void WorkItem(QueueItemDetail p_Item, int p_QueueGroupItemsCount)
        {
            //Vlakno ma nastavene Batch atributy a tak zpracovava davkove a nevola tuto metodu
            throw new NotImplementedException();
        }

        public override void WorkItems(List<QueueItemDetail> p_Item)
        {
            #region pro jistotu kontrola, ze mi QueueWithConsumerThreadD3 spravne roztridil dle IQueueItemGroupId

            QueueItemDetail first = p_Item.First();
            if (p_Item.Exists(i => i.ConnectionString != first.ConnectionString))
            {
                throw new Exception("Nemuzu davkove zpracovat, kdy jsem dostal zaznamy s ruznym ConnectionString");
            }

            #endregion

            if (p_Item.Count > 1)
            {
                DataTable table = new DataTable();
                table.Columns.Add(new DataColumn("HeaderId", typeof(int)));
                table.Columns.Add(new DataColumn("CreatedDate", typeof(DateTime)));
                table.Columns.Add(new DataColumn("StatusId", typeof(int)));
                table.Columns.Add(new DataColumn("Text", typeof(string)));
                p_Item.ForEach(d => table.Rows.Add(d.HeaderId, d.DateTimeNow, (int)d.Status, d.Text));
                using (SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(first.ConnectionString))
                {
                    sqlBulkCopy.DestinationTableName = "SimpleProtocol.Detail";
                    sqlBulkCopy.ColumnMappings.Add("HeaderId", "HeaderId");
                    sqlBulkCopy.ColumnMappings.Add("CreatedDate", "CreatedDate");
                    sqlBulkCopy.ColumnMappings.Add("StatusId", "StatusId");
                    sqlBulkCopy.ColumnMappings.Add("Text", "Text");
                    sqlBulkCopy.WriteToServer(table);
                }
                //Debug.WriteLine($"{DateTime.Now} SimpleProtocol QueueDetail BulkInsert {p_Item.Count} rows");
            }
            else
            {
                using (var conn = new SqlConnection(first.ConnectionString))
                {
                    conn.Open(); 
                    conn.Insert(new DetailRow {HeaderId = first.HeaderId, CreatedDate = first.DateTimeNow, StatusId = (int) first.Status, Text = first.Text});
                }
                //Debug.WriteLine($"{DateTime.Now} SimpleProtocol QueueDetail Insert 1 row");
            }
        }
    }
}