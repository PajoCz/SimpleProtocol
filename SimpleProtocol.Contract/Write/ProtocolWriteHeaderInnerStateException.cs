using System;

namespace SimpleProtocol.Contract.Write
{
    public class ProtocolWriteHeaderInnerStateException : Exception
    {
        public ProtocolWriteHeaderInnerStateException()
        {
        }

        public ProtocolWriteHeaderInnerStateException(string message) : base(message)
        {
        }
    }
}