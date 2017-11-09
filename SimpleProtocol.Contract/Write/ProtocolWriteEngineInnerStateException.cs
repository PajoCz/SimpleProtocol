using System;

namespace SimpleProtocol.Contract.Write
{
    public class ProtocolWriteEngineInnerStateException : Exception
    {
        public ProtocolWriteEngineInnerStateException()
        {
        }

        public ProtocolWriteEngineInnerStateException(string message) : base(message)
        {
        }
    }
}