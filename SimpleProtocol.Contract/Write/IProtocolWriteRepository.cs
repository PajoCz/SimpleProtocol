using System;

namespace SimpleProtocol.Contract.Write
{
    public interface IProtocolWriteRepository<THeaderId, TDetailId>
    {
        THeaderId Start(DateTime p_DateTimeNow, string p_Login, string p_HeaderName);
        //THeaderId StartUniqueLinkedObject(DateTime p_DateTimeNow, string p_Login, string p_HeaderName, LinkedObject p_LinkedObject);
        TDetailId AddDetail(THeaderId p_HeaderId, DateTime p_DateTimeNow, ProtocolStatus p_Status, string p_Text);
        void Stop(THeaderId p_HeaderId, DateTime p_DateTimeNow);
        void AddLinkedObject(THeaderId p_HeaderId, LinkedObject p_LinkedObject);
    }
}