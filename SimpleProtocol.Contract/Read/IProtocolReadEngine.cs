﻿using System.Collections.Generic;

namespace SimpleProtocol.Contract.Read
{
    public interface IProtocolReadEngine<THeaderId>
    {
        IEnumerable<ProtocolHeader<THeaderId>> FindByLinkedObject(LinkedObject p_LinkedObject, bool p_LoadDetails);
    }
}