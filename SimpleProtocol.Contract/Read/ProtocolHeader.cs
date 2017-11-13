using System;
using System.Collections.Generic;
using SimpleProtocol.Contract.Extensions;

namespace SimpleProtocol.Contract.Read
{
    public class ProtocolHeader<THeaderId>
    {
        public THeaderId HeaderId { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedLogin { get; set; }
        public string Name { get; set; }
        public IEnumerable<ProtocolDetail> Details { get; set; }
        public ProtocolStatus? WorstStatusOfChilds()
        {
            if (Details == null) return null;

            ProtocolStatus result = ProtocolStatus.Ok;
            foreach (ProtocolDetail detail in Details)
            {
                result = result.Worst(detail.Status);
            }
            return result;
        }
    }
}