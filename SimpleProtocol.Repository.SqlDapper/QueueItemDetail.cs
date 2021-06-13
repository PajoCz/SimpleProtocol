using System;
using SimpleProtocol.Contract;
using SimpleProtocol.Repository.SqlDapper.ProducerConsumer;

namespace SimpleProtocol.Repository.SqlDapper
{
    public class QueueItemDetail : IQueueItemGroupId
    {
        #region IQueueItemGroupId

        public string QueueGroupId => ConnectionString;

        #endregion

        #region Members

        public string ConnectionString;
        public long HeaderId;
        public DateTime DateTimeNow;
        public ProtocolStatus Status;
        public string Text;

        #endregion
    }
}