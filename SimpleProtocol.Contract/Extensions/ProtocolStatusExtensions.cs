namespace SimpleProtocol.Contract.Extensions
{
    public static class ProtocolStatusExtensions
    {
        public static ProtocolStatus Worst(this ProtocolStatus p_ProtocolStatus, ProtocolStatus p_ProtocolStatus2)
        {
            return (int) p_ProtocolStatus > (int) p_ProtocolStatus2 ? p_ProtocolStatus : p_ProtocolStatus2;
        }
    }
}