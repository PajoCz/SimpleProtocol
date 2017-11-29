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

        [Test]
        public void WorstAddedDetailStatus_ComplexTest()
        {
            var writeHeader = ProtocolWriteHeaderFactoryHelper.ProtocolWriteHeaderWithRepositoryMock();
            Assert.IsNull(writeHeader.WorstAddedDetailStatus);
            writeHeader.Start(null);
            Assert.IsNull(writeHeader.WorstAddedDetailStatus);

            writeHeader.AddDetail(ProtocolStatus.Ok, null);
            Assert.AreEqual(ProtocolStatus.Ok, writeHeader.WorstAddedDetailStatus);

            writeHeader.AddDetail(ProtocolStatus.Warning, null);
            Assert.AreEqual(ProtocolStatus.Warning, writeHeader.WorstAddedDetailStatus);

            writeHeader.AddDetail(ProtocolStatus.Ok, null);
            Assert.AreEqual(ProtocolStatus.Warning, writeHeader.WorstAddedDetailStatus);

            writeHeader.AddDetail(ProtocolStatus.Error, null);
            Assert.AreEqual(ProtocolStatus.Error, writeHeader.WorstAddedDetailStatus);

            writeHeader.AddDetail(ProtocolStatus.Warning, null);
            Assert.AreEqual(ProtocolStatus.Error, writeHeader.WorstAddedDetailStatus);

            writeHeader.AddDetail(ProtocolStatus.Failed, null);
            Assert.AreEqual(ProtocolStatus.Failed, writeHeader.WorstAddedDetailStatus);

            writeHeader.AddDetail(ProtocolStatus.Error, null);
            Assert.AreEqual(ProtocolStatus.Failed, writeHeader.WorstAddedDetailStatus);

            //EndProcess is only for write DateTime as detail - do not modify WorstAddedDetailStatus
            writeHeader.AddDetail(ProtocolStatus.EndProcess, null);
            Assert.AreEqual(ProtocolStatus.Failed, writeHeader.WorstAddedDetailStatus);
        }

        [Test]
        public void WorstAddedDetailStatus_AddDetailEndProcess_NotModifyWirstAddedDetailStatus()
        {
            var writeHeader = ProtocolWriteHeaderFactoryHelper.ProtocolWriteHeaderWithRepositoryMock();
            Assert.IsNull(writeHeader.WorstAddedDetailStatus);
            writeHeader.Start(null);
            Assert.IsNull(writeHeader.WorstAddedDetailStatus);

            //EndProcess is only for write DateTime as detail - do not modify WorstAddedDetailStatus
            writeHeader.AddDetail(ProtocolStatus.EndProcess, null);
            Assert.IsNull(writeHeader.WorstAddedDetailStatus);
        }

        [Test]
        public void WorstAddedDetailStatus_Stop_NotModifyWirstAddedDetailStatus()
        {
            var writeHeader = ProtocolWriteHeaderFactoryHelper.ProtocolWriteHeaderWithRepositoryMock();
            Assert.IsNull(writeHeader.WorstAddedDetailStatus);
            writeHeader.Start(null);
            Assert.IsNull(writeHeader.WorstAddedDetailStatus);

            //EndProcess is only for write DateTime as detail - do not modify WorstAddedDetailStatus
            writeHeader.Stop();
            Assert.IsNull(writeHeader.WorstAddedDetailStatus);
        }
    }
}
