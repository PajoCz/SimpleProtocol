using System;
using System.Data.SqlClient;
using Dapper.Contrib.Extensions;
using SimpleProtocol.Contract;
using SimpleProtocol.Contract.Write;

namespace SimpleProtocol.Repository.SqlDapper
{
    public class ProtocolWriteRepositorySqlDapper : IProtocolWriteRepository<long, long>
    {
        private readonly string _ConnectionString;

        public ProtocolWriteRepositorySqlDapper(string p_ConnectionString)
        {
            _ConnectionString = p_ConnectionString;
        }

        public long Start(DateTime p_DateTimeNow, HeaderEntityWrite p_HeaderEntityWrite)
        {
            using (var conn = new SqlConnection(_ConnectionString))
            {
                conn.Open();
                return conn.Insert(new HeaderRow {CreatedDate = p_DateTimeNow, CreatedLogin = p_HeaderEntityWrite.Login, Name = p_HeaderEntityWrite.HeaderName});
            }
        }

        public long AddDetail(long p_HeaderId, DateTime p_DateTimeNow, DetailEntityWrite p_DetailEntityWrite)
        {
            using (var conn = new SqlConnection(_ConnectionString))
            {
                conn.Open();
                return conn.Insert(new DetailRow {HeaderId = p_HeaderId, CreatedDate = p_DateTimeNow, Status = (int) p_DetailEntityWrite.Status, Text = p_DetailEntityWrite.Text});
            }
        }

        public void Stop(long p_HeaderId, DateTime p_DateTimeNow)
        {
            using (var conn = new SqlConnection(_ConnectionString))
            {
                conn.Open();
                conn.Insert(new DetailRow { HeaderId = p_HeaderId, CreatedDate = p_DateTimeNow, Status = (int)ProtocolStatus.EndProcess});
            }
        }

        public void AddLinkedObject(long p_HeaderId, LinkedObject p_LinkedObject)
        {
            using (var conn = new SqlConnection(_ConnectionString))
            {
                conn.Open();
                conn.Insert(new LinkedObjectRow() { HeaderId = p_HeaderId, Name = p_LinkedObject.ObjectName, Id = p_LinkedObject.ObjectId.ToString()});
            }
        }
    }
}