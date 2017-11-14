using SimpleProtocol.Contract;
using SimpleProtocol.Contract.Write;

namespace SimpleProtocol.Engine.Write
{
    public class ProtocolWriteEngineFactory<THeaderId> : IProtocolWriteEngineFactory<THeaderId>
    {
        private readonly IDateTime _DateTime;
        private readonly ILogin _Login;
        private readonly IProtocolWriteRepository<THeaderId> _ProtocolWriteRepository;

        public ProtocolWriteEngineFactory(IDateTime p_DateTime, ILogin p_Login, IProtocolWriteRepository<THeaderId> p_ProtocolWriteRepository)
        {
            _DateTime = p_DateTime;
            _Login = p_Login;
            _ProtocolWriteRepository = p_ProtocolWriteRepository;
        }

        public IProtocolWriteEngine<THeaderId> Create()
        {
            return new ProtocolWriteEngine<THeaderId>(_DateTime, _Login, _ProtocolWriteRepository);
        }
    }
}