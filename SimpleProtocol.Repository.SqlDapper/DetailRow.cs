using System;
using Dapper.Contrib.Extensions;
using SimpleProtocol.Contract;
using SimpleProtocol.Contract.Read;

namespace SimpleProtocol.Repository.SqlDapper
{
    [Table("SimpleProtocol.Detail")]
    public class DetailRow
    {
        [Key]
        public long DetailId { get; set; }
        public long HeaderId { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedLogin { get; set; }
        public int Status { get; set; }
        public string Text { get; set; }

        public ProtocolDetail ToProtocolDetail()
        {
            return new ProtocolDetail()
            {
                CreatedDate = CreatedDate,
                CreatedLogin = CreatedLogin,
                Status = (ProtocolStatus) Status,
                Text = Text,
            };
        }
    }
}