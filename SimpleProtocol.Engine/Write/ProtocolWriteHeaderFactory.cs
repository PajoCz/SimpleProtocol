using SimpleProtocol.Contract;
using SimpleProtocol.Contract.Write;

namespace SimpleProtocol.Engine.Write
{
    public class ProtocolWriteHeaderFactory<THeaderId, TDetailId> : IProtocolWriteHeaderFactory<THeaderId, TDetailId>
    {
        private readonly IDateTime _DateTime;
        private readonly ILogin _Login;
        private readonly IProtocolWriteRepository<THeaderId, TDetailId> _ProtocolWriteRepository;

        public ProtocolWriteHeaderFactory(IDateTime p_DateTime, ILogin p_Login, IProtocolWriteRepository<THeaderId, TDetailId> p_ProtocolWriteRepository)
        {
            _DateTime = p_DateTime;
            _Login = p_Login;
            _ProtocolWriteRepository = p_ProtocolWriteRepository;
        }

        private IProtocolWriteHeader<THeaderId, TDetailId> CreateImpl(string p_HeaderName, bool p_AutoStop, LinkedObject p_LinkedObject = null)
        {
            var result = new ProtocolWriteHeader<THeaderId, TDetailId>(_DateTime, _Login, _ProtocolWriteRepository, p_AutoStop);
            result.Start(p_HeaderName);
            if (p_LinkedObject != null)
            {
                result.AddLinkedObject(p_LinkedObject);
            }
            return result;
        }

        public IProtocolWriteHeader<THeaderId, TDetailId> Create(string p_HeaderName, LinkedObject p_LinkedObject = null)
        {
            return CreateImpl(p_HeaderName, false, p_LinkedObject);
        }

        public IProtocolWriteHeader<THeaderId, TDetailId> CreateAutoStop(string p_HeaderName, LinkedObject p_LinkedObject = null)
        {
            return CreateImpl(p_HeaderName, true, p_LinkedObject);
        }

    }
}