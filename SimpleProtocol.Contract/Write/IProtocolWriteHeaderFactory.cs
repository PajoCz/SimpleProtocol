namespace SimpleProtocol.Contract.Write
{
    public interface IProtocolWriteHeaderFactory<THeaderId, TDetailId>
    {
        IProtocolWriteHeader<THeaderId, TDetailId> Create(string p_HeaderName, LinkedObject p_LinkedObject = null);
        IProtocolWriteHeader<THeaderId, TDetailId> CreateAutoStop(string p_HeaderName, LinkedObject p_LinkedObject = null);
    }
}