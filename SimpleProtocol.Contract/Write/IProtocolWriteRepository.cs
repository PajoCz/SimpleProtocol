using System;

namespace SimpleProtocol.Contract.Write
{
    public interface IProtocolWriteRepository<THeaderId, TDetailId>
    {
        THeaderId Start(DateTime p_DateTimeNow, HeaderEntityWrite p_HeaderEntityWrite);
        TDetailId AddDetail(THeaderId p_HeaderId, DateTime p_DateTimeNow, DetailEntityWrite p_DetailEntityWrite);
        void Stop(THeaderId p_HeaderId, DateTime p_DateTimeNow);
        void AddLinkedObject(THeaderId p_HeaderId, LinkedObject p_LinkedObject);
    }
}