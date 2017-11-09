using System;
using Dapper.Contrib.Extensions;

namespace SimpleProtocol.Repository.SqlDapper
{
    [Table("SimpleProtocol.Detail")]
    public class DetailRow
    {
        [Key]
        public long DetailId { get; set; }
        public long HeaderId { get; set; }
        public DateTime CreatedDate { get; set; }
        public int Status { get; set; }
        public string Text { get; set; }
    }
}