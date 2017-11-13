using System.Collections.Generic;
using SimpleProtocol.Contract;
using SimpleProtocol.Contract.Read;

namespace SimpleProtocol.Engine.Read
{
    public class ProtocolReadEngine : IProtocolReadEngine<long>
    {
        private readonly IProtocolReadRepository<long> _ProtocolReadRepository;

        public ProtocolReadEngine(IProtocolReadRepository<long> p_ProtocolReadRepository)
        {
            _ProtocolReadRepository = p_ProtocolReadRepository;
        }

        public IEnumerable<ProtocolHeader<long>> FindByLinkedObject(LinkedObject p_LinkedObject, bool p_LoadDetails)
        {
            return _ProtocolReadRepository.FindByLinkedObject(p_LinkedObject, p_LoadDetails);
        }
    }
}