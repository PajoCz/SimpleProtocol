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
        public int StatusId { get; set; }
        public string Text { get; set; }

        public ProtocolDetail ToProtocolDetail()
        {
            return new ProtocolDetail()
            {
                CreatedDate = CreatedDate,
                Status = (ProtocolStatus) StatusId,
                Text = Text,
            };
        }
    }
}