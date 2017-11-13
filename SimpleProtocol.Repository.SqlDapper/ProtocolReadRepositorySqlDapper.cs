using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Dapper.Contrib.Extensions;
using SimpleProtocol.Contract;
using SimpleProtocol.Contract.Read;

namespace SimpleProtocol.Repository.SqlDapper
{
    public class ProtocolReadRepositorySqlDapper : IProtocolReadRepository<long>
    {
        private readonly string _ConnectionString;

        public ProtocolReadRepositorySqlDapper(string p_ConnectionString)
        {
            _ConnectionString = p_ConnectionString;
        }

        public IEnumerable<ProtocolHeader<long>> FindByLinkedObject(LinkedObject p_LinkedObject, bool p_LoadDetails)
        {
            using (var conn = new SqlConnection(_ConnectionString))
            {
                conn.Open();
                var found = conn.Query<long>("SELECT HeaderId FROM SimpleProtocol.LinkedObject WHERE Name=@Name AND Id=@Id",
                    new { Name = p_LinkedObject.ObjectName, Id = p_LinkedObject.ObjectId }).ToList();
                var result = conn.Query<HeaderRow>($"SELECT * FROM SimpleProtocol.Header WHERE HeaderId IN ({string.Join(", ", found)})").ToList().ConvertAll(i => i.ToProtocolHeader());

                if (p_LoadDetails && found.Any())
                {
                    var foundDetails = conn.Query<DetailRow>($"SELECT * FROM SimpleProtocol.Detail WHERE HeaderId IN ({string.Join(", ", found)})").ToList();
                    foundDetails.ForEach(d =>
                    {
                        var headerForDetail = result.Find(r => r.HeaderId == d.HeaderId);
                        (headerForDetail.Details as IList<ProtocolDetail>).Add(d.ToProtocolDetail());
                    });
                }

                return result;
            }
        }
    }
}
