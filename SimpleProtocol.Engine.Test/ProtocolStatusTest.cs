using NUnit.Framework;
using SimpleProtocol.Contract;
using SimpleProtocol.Contract.Extensions;

namespace SimpleProtocol.Engine.Test
{
    [TestFixture]
    public class ProtocolStatusTest
    {
        [Test]
        public void WorstExtension()
        {
            ProtocolStatus status = ProtocolStatus.Ok;
            Assert.AreEqual(ProtocolStatus.Ok, status.Worst(ProtocolStatus.Ok));
            Assert.AreEqual(ProtocolStatus.Warning, status.Worst(ProtocolStatus.Warning));
            Assert.AreEqual(ProtocolStatus.Error, status.Worst(ProtocolStatus.Error));
            Assert.AreEqual(ProtocolStatus.Failed, status.Worst(ProtocolStatus.Failed));
            Assert.AreEqual(ProtocolStatus.Ok, status.Worst(ProtocolStatus.Info));

            status = ProtocolStatus.Warning;
            Assert.AreEqual(ProtocolStatus.Warning, status.Worst(ProtocolStatus.Ok));
            Assert.AreEqual(ProtocolStatus.Warning, status.Worst(ProtocolStatus.Warning));
            Assert.AreEqual(ProtocolStatus.Error, status.Worst(ProtocolStatus.Error));
            Assert.AreEqual(ProtocolStatus.Failed, status.Worst(ProtocolStatus.Failed));
            Assert.AreEqual(ProtocolStatus.Warning, status.Worst(ProtocolStatus.Info));

            status = ProtocolStatus.Error;
            Assert.AreEqual(ProtocolStatus.Error, status.Worst(ProtocolStatus.Ok));
            Assert.AreEqual(ProtocolStatus.Error, status.Worst(ProtocolStatus.Warning));
            Assert.AreEqual(ProtocolStatus.Error, status.Worst(ProtocolStatus.Error));
            Assert.AreEqual(ProtocolStatus.Failed, status.Worst(ProtocolStatus.Failed));
            Assert.AreEqual(ProtocolStatus.Error, status.Worst(ProtocolStatus.Info));

            status = ProtocolStatus.Failed;
            Assert.AreEqual(ProtocolStatus.Failed, status.Worst(ProtocolStatus.Ok));
            Assert.AreEqual(ProtocolStatus.Failed, status.Worst(ProtocolStatus.Error));
            Assert.AreEqual(ProtocolStatus.Failed, status.Worst(ProtocolStatus.Warning));
            Assert.AreEqual(ProtocolStatus.Failed, status.Worst(ProtocolStatus.Failed));
            Assert.AreEqual(ProtocolStatus.Failed, status.Worst(ProtocolStatus.Info));

            status = ProtocolStatus.Info;
            Assert.AreEqual(ProtocolStatus.Ok, status.Worst(ProtocolStatus.Ok));
            Assert.AreEqual(ProtocolStatus.Error, status.Worst(ProtocolStatus.Error));
            Assert.AreEqual(ProtocolStatus.Warning, status.Worst(ProtocolStatus.Warning));
            Assert.AreEqual(ProtocolStatus.Failed, status.Worst(ProtocolStatus.Failed));
            Assert.AreEqual(ProtocolStatus.Info, status.Worst(ProtocolStatus.Info));
        }
    }
}
