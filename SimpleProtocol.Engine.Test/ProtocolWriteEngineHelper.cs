using Moq;
using SimpleProtocol.Contract.Write;
using SimpleProtocol.Engine.Write;

namespace SimpleProtocol.Engine.Test
{
    public class ProtocolWriteEngineHelper
    {
        public static IProtocolWriteEngine<long> ProtocolWriteEngineWithRepositoryMock()
        {
            var protocolWriteRepositoryMock = new Mock<IProtocolWriteRepository<long>>().Object;
            var writeEngine = new ProtocolWriteEngine(new DateTimeDefaultImpl(), protocolWriteRepositoryMock);
            return writeEngine;
        }
    }
}