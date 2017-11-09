using System.Collections.Generic;

namespace SimpleProtocol.Contract.Read
{
    public interface IProtocolReadEngine<THeaderId, TDetailId>
    {
        IEnumerable<ProtocolHeader<THeaderId, TDetailId>> FindBySearchableObject(LinkedObject p_LinkedObject, bool p_LoadDetails);
    }
}