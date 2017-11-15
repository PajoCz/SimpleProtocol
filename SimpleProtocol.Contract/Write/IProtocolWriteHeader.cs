using System;

namespace SimpleProtocol.Contract.Write
{
    public interface IProtocolWriteHeader<THeaderId> : IDisposable
    {
        ProtocolWriteHeaderInnerState InnerState { get; }
        string Login { get; set; }
        THeaderId HeaderId { get; }
        THeaderId Start(string p_HeaderName, LinkedObject p_LinkedObject = null);
        //THeaderId StartUniqueLinkedObject(string p_HeaderName, LinkedObject p_LinkedObject);
        void AddDetail(ProtocolStatus p_Status, string p_Text);
        void AddLinkedObject(LinkedObject p_LinkedObject);
        void Stop();
    }
}