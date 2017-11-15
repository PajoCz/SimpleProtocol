namespace SimpleProtocol.Contract.Write
{
    public interface IProtocolWriteHeaderFactory<THeaderId>
    {
        IProtocolWriteHeader<THeaderId> Create(string p_HeaderName, LinkedObject p_LinkedObject = null);
        IProtocolWriteHeader<THeaderId> CreateAutoStop(string p_HeaderName, LinkedObject p_LinkedObject = null);
    }
}