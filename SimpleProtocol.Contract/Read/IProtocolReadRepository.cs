using System.Collections.Generic;

namespace SimpleProtocol.Contract.Read
{
    public interface IProtocolReadRepository<THeaderId, TDetailId>
    {
        IEnumerable<ProtocolHeader<THeaderId, TDetailId>> FindByLinkedObject(LinkedObject p_LinkedObject, bool p_LoadDetails);
    }
}