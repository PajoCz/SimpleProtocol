using SimpleProtocol.Contract.Write;

namespace SimpleProtocol.Engine.Write
{
    public class ProtocolWriteEngineAutoStartStop<THeaderId> : IProtocolWriteEngineAutoStartStop
    {
        private readonly IProtocolWriteEngine<THeaderId> _Owner;

        public ProtocolWriteEngineAutoStartStop(IProtocolWriteEngine<THeaderId> p_Owner, HeaderEntityWrite p_HeaderEntityWrite)
        {
            _Owner = p_Owner;
            _Owner.Start(p_HeaderEntityWrite);
        }

        public void Dispose()
        {
            _Owner.Stop();
        }
    }
}