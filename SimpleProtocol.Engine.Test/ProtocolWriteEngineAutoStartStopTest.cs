using NUnit.Framework;
using SimpleProtocol.Contract;
using SimpleProtocol.Contract.Write;

namespace SimpleProtocol.Engine.Test
{
    [TestFixture]
    public class ProtocolWriteEngineAutoStartStopTest
    {
        [Test]
        public void CreateAutoStartStop_CheckInnerState()
        {
            var writeEngine = ProtocolWriteEngineHelper.ProtocolWriteEngineWithRepositoryMock();
            Assert.AreEqual(ProtocolWriteEngineInnerState.Created, writeEngine.InnerState);
            using (writeEngine.CreateAutoStartStop(null))
            {
                Assert.AreEqual(ProtocolWriteEngineInnerState.Started, writeEngine.InnerState);
                writeEngine.AddDetail(ProtocolStatus.Ok, null);
            }
            Assert.AreEqual(ProtocolWriteEngineInnerState.Stopped, writeEngine.InnerState);
        }

        [Test]
        public void CreateAutoStartStop_AfterStartedEngine_Throws()
        {
            var writeEngine = ProtocolWriteEngineHelper.ProtocolWriteEngineWithRepositoryMock();
            writeEngine.Start(null);
            Assert.Throws<ProtocolWriteEngineInnerStateException>(() => writeEngine.CreateAutoStartStop(null));
        }

        [Test]
        public void CreateAutoStartStop_StopUsedInnerOfCreatedHandle_Throws()
        {
            var writeEngine = ProtocolWriteEngineHelper.ProtocolWriteEngineWithRepositoryMock();
            Assert.Throws<ProtocolWriteEngineInnerStateException>(() =>
            {
                using (writeEngine.CreateAutoStartStop(null))
                {
                    writeEngine.Stop();
                }
            });
        }
    }
}
