namespace SimpleProtocol.Contract.Write
{
    public interface IProtocolWriteEngine<THeaderId>
    {
        ProtocolWriteEngineInnerState InnerState { get; }
        THeaderId HeaderId { get; }
        THeaderId Start(HeaderEntityWrite p_HeaderEntityWrite);
        THeaderId StartUniqueLinkedObject(HeaderEntityWrite p_HeaderEntityWrite, LinkedObject p_LinkedObject);
        void AddDetail(DetailEntityWrite p_DetailEntityWrite);
        void AddLinkedObject(LinkedObject p_LinkedObject);
        void Stop();
        IProtocolWriteEngineAutoStartStop CreateAutoStartStop(HeaderEntityWrite p_HeaderEntityWrite);
    }
}