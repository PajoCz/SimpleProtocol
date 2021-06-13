using System;
using System.Data.SqlClient;
using System.Linq;
using Dapper;
using Dapper.Contrib.Extensions;
using SimpleProtocol.Contract;
using SimpleProtocol.Contract.Write;
using SimpleProtocol.Repository.SqlDapper.ProducerConsumer;

namespace SimpleProtocol.Repository.SqlDapper
{
    public class ProtocolWriteRepositorySqlDapper : IProtocolWriteRepository<long, long>
    {
        private readonly string _ConnectionString;
        private readonly bool _UseInMemoryCacheDetailAndBulkInsert;
        private static readonly QueueDetail _CacheDetail = new QueueDetail() { WorkBatchItems = 1000, WorkBatchTimeout = TimeSpan.FromSeconds(1)};

        //Optional dependency injected class
        public IConsumerThreadExceptionProcessing ConsumerThreadExceptionProcessing { get; set; }

        public ProtocolWriteRepositorySqlDapper(string p_ConnectionString, bool p_UseInMemoryCacheDetailAndBulkInsert = false)
        {
            _ConnectionString = p_ConnectionString;
            _UseInMemoryCacheDetailAndBulkInsert = p_UseInMemoryCacheDetailAndBulkInsert;
        }

        public long Start(DateTime p_DateTimeNow, string p_Login, string p_HeaderName)
        {
            using (var conn = new SqlConnection(_ConnectionString))
            {
                conn.Open();
                return conn.Insert(new HeaderRow {CreatedDate = p_DateTimeNow, CreatedLogin = p_Login, Name = p_HeaderName});
            }
        }

        //public long StartUniqueLinkedObject(DateTime p_DateTimeNow, string p_Login, string p_HeaderName, LinkedObject p_LinkedObject)
        //{
        //    using (var conn = new SqlConnection(_ConnectionString))
        //    {
        //        conn.Open();

        //        //Posibility 1 : By T-SQL
        //        var found = conn.Query<long>("SELECT HeaderId FROM SimpleProtocol.LinkedObject WHERE Name=@Name AND Id=@Id",
        //            new { Name = p_LinkedObject.ObjectName, Id = p_LinkedObject.ObjectId }).ToList();

        //        //Posibility 2 : By Linq (by nuget package 'MicroOrm.Dapper.Repositories')
        //        //var repo = new DapperRepository<LinkedObjectRow>(conn);
        //        //var found = repo.FindAll(i => i.Name == p_LinkedObject.ObjectName && i.Id == p_LinkedObject.ObjectId.ToString()).ToList();

        //        if (found.Count > 1)
        //        {
        //            throw new Exception($"More HeaderId found in LinkedObject with Name={p_LinkedObject.ObjectName} AND Id={p_LinkedObject.ObjectId}");
        //        }

        //        if (found.Count == 1)
        //        {
        //            return found.FirstOrDefault();
        //        }

        //        //create new with linked object
        //        var result = conn.Insert(new HeaderRow { CreatedDate = p_DateTimeNow, CreatedLogin = p_Login, Name = p_HeaderName });
        //        conn.Insert(new LinkedObjectRow() { HeaderId = result, Name = p_LinkedObject.ObjectName, Id = p_LinkedObject.ObjectId.ToString() });
        //        return result;
        //    }
        //}

        public long AddDetail(long p_HeaderId, DateTime p_DateTimeNow, ProtocolStatus p_Status, string p_Text)
        {
            if (_UseInMemoryCacheDetailAndBulkInsert)
            {
                if (_CacheDetail.ExceptionProcessing == null)
                    _CacheDetail.ExceptionProcessing = ConsumerThreadExceptionProcessing;
                _CacheDetail.AddItem(new QueueItemDetail()
                {
                    ConnectionString = _ConnectionString,
                    DateTimeNow = p_DateTimeNow,
                    HeaderId = p_HeaderId,
                    Status = p_Status,
                    Text = p_Text
                });
                return 0;
            }

            using (var conn = new SqlConnection(_ConnectionString))
            {
                conn.Open();
                return conn.Insert(new DetailRow {HeaderId = p_HeaderId, CreatedDate = p_DateTimeNow, StatusId = (int) p_Status, Text = p_Text});
            }
        }

        public void Stop(long p_HeaderId, DateTime p_DateTimeNow)
        {
            AddDetail(p_HeaderId, p_DateTimeNow, ProtocolStatus.EndProcess, null);
        }

        public void AddLinkedObject(long p_HeaderId, LinkedObject p_LinkedObject)
        {
            using (var conn = new SqlConnection(_ConnectionString))
            {
                conn.Open();
                conn.Insert(new LinkedObjectRow() { HeaderId = p_HeaderId, Name = p_LinkedObject.ObjectName, Id = p_LinkedObject.ObjectId?.ToString()});
            }
        }
    }
}