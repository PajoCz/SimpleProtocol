using Moq;
using SimpleProtocol.Contract.Write;
using SimpleProtocol.Engine.Write;

namespace SimpleProtocol.Engine.Test
{
    public class ProtocolWriteHeaderFactoryHelper
    {
        public static IProtocolWriteHeaderFactory<long, long> ProtocolWriteHeaderFactoryWithRepositoryMock()
        {
            var protocolWriteRepositoryMock = new Mock<IProtocolWriteRepository<long, long>>().Object;
            var writeHeaderFactory = new ProtocolWriteHeaderFactory<long, long>(new DateTimeDefaultImpl(), new LoginNullImpl(), protocolWriteRepositoryMock);
            return writeHeaderFactory;
        }

        public static IProtocolWriteHeader<long, long> ProtocolWriteHeaderWithRepositoryMock()
        {
            var protocolWriteRepositoryMock = new Mock<IProtocolWriteRepository<long, long>>().Object;
            var writeHeader = new ProtocolWriteHeader<long, long>(new DateTimeDefaultImpl(), new LoginNullImpl(), protocolWriteRepositoryMock, false);
            return writeHeader;
        }
    }
}