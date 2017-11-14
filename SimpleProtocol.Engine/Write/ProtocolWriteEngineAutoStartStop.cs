using SimpleProtocol.Contract;
using SimpleProtocol.Contract.Write;

namespace SimpleProtocol.Engine.Write
{
    public class ProtocolWriteEngineAutoStartStop<THeaderId> : IProtocolWriteEngineAutoStartStop
    {
        private readonly IProtocolWriteEngine<THeaderId> _Owner;
        private readonly bool _StopAtDisable;

        public ProtocolWriteEngineAutoStartStop(IProtocolWriteEngine<THeaderId> p_Owner, string p_HeaderName)
        {
            _Owner = p_Owner;
            _Owner.Start(p_HeaderName);
        }

        public void Dispose()
        {
            _Owner.Stop();
        }

        public IProtocolWriteEngineAutoStartStop AddLinkedObject(LinkedObject p_LinkedObject)
        {
            _Owner.AddLinkedObject(p_LinkedObject);
            return this;
        }
    }
}