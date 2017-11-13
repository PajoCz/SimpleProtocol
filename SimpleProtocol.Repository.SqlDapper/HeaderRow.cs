using System;
using System.Collections.Generic;
using Dapper.Contrib.Extensions;
using SimpleProtocol.Contract.Read;

namespace SimpleProtocol.Repository.SqlDapper
{
    [Table("SimpleProtocol.Header")]
    public class HeaderRow
    {
        [Key]
        public long HeaderId { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedLogin { get; set; }
        public string Name { get; set; }

        public ProtocolHeader<long> ToProtocolHeader()
        {
            return new ProtocolHeader<long>()
            {
                HeaderId = HeaderId,
                CreatedDate = CreatedDate,
                CreatedLogin = CreatedLogin,
                Name = Name,
                Details = new List<ProtocolDetail>()
            };
        }
    }
}