using System;

namespace SimpleProtocol.Contract.Write
{
    public interface IProtocolWriteRepository<THeaderId>
    {
        THeaderId Start(DateTime p_DateTimeNow, HeaderEntityWrite p_HeaderEntityWrite);
        THeaderId StartUniqueLinkedObject(DateTime p_DateTimeNow, HeaderEntityWrite p_HeaderEntityWrite, LinkedObject p_LinkedObject);
        void AddDetail(THeaderId p_HeaderId, DateTime p_DateTimeNow, DetailEntityWrite p_DetailEntityWrite);
        void Stop(THeaderId p_HeaderId, DateTime p_DateTimeNow);
        void AddLinkedObject(THeaderId p_HeaderId, LinkedObject p_LinkedObject);
    }
}