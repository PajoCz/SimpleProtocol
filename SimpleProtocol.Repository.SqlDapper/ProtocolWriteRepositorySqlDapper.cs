using System;
using System.Data.SqlClient;
using System.Linq;
using Dapper;
using Dapper.Contrib.Extensions;
using SimpleProtocol.Contract;
using SimpleProtocol.Contract.Write;

namespace SimpleProtocol.Repository.SqlDapper
{
    public class ProtocolWriteRepositorySqlDapper : IProtocolWriteRepository<long>
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

        public long StartUniqueLinkedObject(DateTime p_DateTimeNow, HeaderEntityWrite p_HeaderEntityWrite, LinkedObject p_LinkedObject)
        {
            using (var conn = new SqlConnection(_ConnectionString))
            {
                conn.Open();

                //Posibility 1 : By T-SQL
                var found = conn.Query<long>("SELECT HeaderId FROM SimpleProtocol.LinkedObject WHERE Name=@Name AND Id=@Id",
                    new { Name = p_LinkedObject.ObjectName, Id = p_LinkedObject.ObjectId }).ToList();

                //Posibility 2 : By Linq (by nuget package 'MicroOrm.Dapper.Repositories')
                //var repo = new DapperRepository<LinkedObjectRow>(conn);
                //var found = repo.FindAll(i => i.Name == p_LinkedObject.ObjectName && i.Id == p_LinkedObject.ObjectId.ToString()).ToList();

                if (found.Count > 1)
                {
                    throw new Exception($"More HeaderId found in LinkedObject with Name={p_LinkedObject.ObjectName} AND Id={p_LinkedObject.ObjectId}");
                }

                if (found.Count == 1)
                {
                    return found.FirstOrDefault();
                }

                //create new with linked object
                var result = conn.Insert(new HeaderRow { CreatedDate = p_DateTimeNow, CreatedLogin = p_HeaderEntityWrite.Login, Name = p_HeaderEntityWrite.HeaderName });
                conn.Insert(new LinkedObjectRow() { HeaderId = result, Name = p_LinkedObject.ObjectName, Id = p_LinkedObject.ObjectId.ToString() });
                return result;
            }
        }

        public void AddDetail(long p_HeaderId, DateTime p_DateTimeNow, DetailEntityWrite p_DetailEntityWrite)
        {
            using (var conn = new SqlConnection(_ConnectionString))
            {
                conn.Open();
                conn.Insert(new DetailRow {HeaderId = p_HeaderId, CreatedDate = p_DateTimeNow, Status = (int) p_DetailEntityWrite.Status, Text = p_DetailEntityWrite.Text});
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