namespace SimpleProtocol.Contract.Write
{
    public interface IProtocolWriteEngineFactory<THeaderId>
    {
        IProtocolWriteEngine<THeaderId> Create();
    }
}