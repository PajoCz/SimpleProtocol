using SimpleProtocol.Contract.Write;

namespace SimpleProtocol.Engine.Write
{
    public class ProtocolWriteEngineAutoStartStop<THeaderId, TDetailId> : IProtocolWriteEngineAutoStartStop
    {
        private readonly IProtocolWriteEngine<THeaderId, TDetailId> _Owner;

        public ProtocolWriteEngineAutoStartStop(IProtocolWriteEngine<THeaderId, TDetailId> p_Owner, HeaderEntityWrite p_HeaderEntityWrite)
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