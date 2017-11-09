namespace SimpleProtocol.Contract.Write
{
    public interface IProtocolWriteEngine<THeaderId, TDetailId>
    {
        ProtocolWriteEngineInnerState InnerState { get; }
        THeaderId HeaderId { get; }
        THeaderId Start(HeaderEntityWrite p_HeaderEntityWrite);
        TDetailId AddDetail(DetailEntityWrite p_DetailEntityWrite);
        void AddLinkedObject(LinkedObject p_LinkedObject);
        void Stop();
        IProtocolWriteEngineAutoStartStop CreateAutoStartStop(HeaderEntityWrite p_HeaderEntityWrite);
    }
}