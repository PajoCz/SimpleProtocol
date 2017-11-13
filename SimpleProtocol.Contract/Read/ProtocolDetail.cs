using System;

namespace SimpleProtocol.Contract.Read
{
    public class ProtocolDetail
    {
        public DateTime CreatedDate { get; set; }
        public string CreatedLogin { get; set; }
        public ProtocolStatus Status { get; set; }
        public string Text { get; set; }

    }
}