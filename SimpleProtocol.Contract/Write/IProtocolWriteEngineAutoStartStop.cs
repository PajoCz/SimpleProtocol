using System;

namespace SimpleProtocol.Contract.Write
{
    public interface IProtocolWriteEngineAutoStartStop : IDisposable
    {
        IProtocolWriteEngineAutoStartStop AddLinkedObject(LinkedObject p_LinkedObject);
    }
}