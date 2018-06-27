using System;

namespace SimpleProtocol.Contract.Read
{
    public class ProtocolDetail<TDetailId>
    {
        public TDetailId DetailId { get; set; }
        public DateTime CreatedDate { get; set; }
        public ProtocolStatus Status { get; set; }
        public string Text { get; set; }

    }
}