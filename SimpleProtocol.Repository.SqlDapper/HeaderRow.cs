using System;
using Dapper.Contrib.Extensions;

namespace SimpleProtocol.Repository.SqlDapper
{
    [Table("SimpleProtocol.Header")]
    public class HeaderRow
    {
        [Key]
        public int HeaderId { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedLogin { get; set; }
        public string Name { get; set; }
    }
}