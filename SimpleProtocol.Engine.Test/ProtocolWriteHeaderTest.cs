using NUnit.Framework;
using SimpleProtocol.Contract;
using SimpleProtocol.Contract.Write;

namespace SimpleProtocol.Engine.Test
{
    [TestFixture]
    public class ProtocolWriteHeaderTest
    {
        [Test]
        public void Created_WithoutStart_CalledAddDetail_CalledAddLinkedObject_ThrowsException()
        {
            var writeHeader = ProtocolWriteHeaderFactoryHelper.ProtocolWriteHeaderWithRepositoryMock();
            Assert.Throws<ProtocolWriteHeaderInnerStateException>(() => writeHeader.AddDetail(ProtocolStatus.Ok, null));
            Assert.Throws<ProtocolWriteHeaderInnerStateException>(() => writeHeader.AddLinkedObject(null));
        }

        [Test]
        public void CheckInnerState_AfterCreate_AfterStart_AfterStop()
        {
            var writeHeader = ProtocolWriteHeaderFactoryHelper.ProtocolWriteHeaderWithRepositoryMock();
            Assert.AreEqual(ProtocolWriteHeaderInnerState.Created, writeHeader.InnerState);
            writeHeader.Start(null);
            Assert.AreEqual(ProtocolWriteHeaderInnerState.Started, writeHeader.InnerState);
            writeHeader.Stop();
            Assert.AreEqual(ProtocolWriteHeaderInnerState.Stopped, writeHeader.InnerState);
        }

        [Test]
        public void Created_CalledStart_CalledAddDetail_CalledAddLinkedObject_CalledStop_Ok()
        {
            var writeHeader = ProtocolWriteHeaderFactoryHelper.ProtocolWriteHeaderWithRepositoryMock();
            writeHeader.Start(null);
            writeHeader.AddDetail(ProtocolStatus.Ok, null);
            writeHeader.AddLinkedObject(null);
            writeHeader.Stop();
        }
    }
}
