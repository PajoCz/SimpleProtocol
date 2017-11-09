using System;
using Dapper.Contrib.Extensions;

namespace SimpleProtocol.Repository.SqlDapper
{
    [Table("SimpleProtocol.LinkedObject")]
    public class LinkedObjectRow
    {
        [Key]
        public long LinkedObjectId { get; set; }
        public long HeaderId { get; set; }
        public string Name { get; set; }
        public string Id { get; set; }
    }
}