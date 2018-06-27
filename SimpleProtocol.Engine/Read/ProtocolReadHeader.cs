using System.Collections.Generic;
using SimpleProtocol.Contract;
using SimpleProtocol.Contract.Read;

namespace SimpleProtocol.Engine.Read
{
    public class ProtocolReadHeader : IProtocolReadHeader<long, long>
    {
        private readonly IProtocolReadRepository<long, long> _ProtocolReadRepository;

        public ProtocolReadHeader(IProtocolReadRepository<long, long> p_ProtocolReadRepository)
        {
            _ProtocolReadRepository = p_ProtocolReadRepository;
        }

        public IEnumerable<ProtocolHeader<long, long>> FindByLinkedObject(LinkedObject p_LinkedObject, bool p_LoadDetails)
        {
            return _ProtocolReadRepository.FindByLinkedObject(p_LinkedObject, p_LoadDetails);
        }
    }
}