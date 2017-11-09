namespace SimpleProtocol.Contract.Read
{
    public class ProtocolDetail<TDetailId>
    {
        public TDetailId DetailId { get; set; }
        public ProtocolStatus Status { get; set; }

    }
}