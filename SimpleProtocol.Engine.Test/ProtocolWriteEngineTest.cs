using NUnit.Framework;
using SimpleProtocol.Contract.Write;

namespace SimpleProtocol.Engine.Test
{
    [TestFixture]
    public class ProtocolWriteEngineTest
    {
        [Test]
        public void Created_WithoutStart_CalledAddDetail_CalledAddLinkedObject_ThrowsException()
        {
            var writeEngine = ProtocolWriteEngineHelper.ProtocolWriteEngineWithRepositoryMock();
            Assert.Throws<ProtocolWriteEngineInnerStateException>(() => writeEngine.AddDetail(null));
            Assert.Throws<ProtocolWriteEngineInnerStateException>(() => writeEngine.AddLinkedObject(null));
        }

        [Test]
        public void CheckInnerState_AfterCreate_AfterStart_AfterStop()
        {
            var writeEngine = ProtocolWriteEngineHelper.ProtocolWriteEngineWithRepositoryMock();
            Assert.AreEqual(ProtocolWriteEngineInnerState.Created, writeEngine.InnerState);
            writeEngine.Start(null);
            Assert.AreEqual(ProtocolWriteEngineInnerState.Started, writeEngine.InnerState);
            writeEngine.Stop();
            Assert.AreEqual(ProtocolWriteEngineInnerState.Stopped, writeEngine.InnerState);
        }

        [Test]
        public void Created_WithStart_CalledAddDetail_CalledAddLinkedObject_Ok()
        {
            var writeEngine = ProtocolWriteEngineHelper.ProtocolWriteEngineWithRepositoryMock();
            writeEngine.Start(null);
            writeEngine.AddDetail(null);
            writeEngine.AddLinkedObject(null);
            writeEngine.Stop();
        }
    }
}
