using System;
using NUnit.Framework;
using SimpleProtocol.Contract;
using SimpleProtocol.Contract.Write;
using SimpleProtocol.Engine.Repository;
using SimpleProtocol.Engine.Write;

namespace SimpleProtocol.Engine.Test
{
    [TestFixture]
    public class ProtocolWriteEngineWithRepositoryFileTest
    {
        [Test]
        public void Start_AddDetail_Stop()
        {
            var writeEngine = ProtocolWriteEngineWithRepositoryFile();
            using (writeEngine.CreateAutoStartStop("Test protocol 1"))
            {
                writeEngine.AddDetail(ProtocolStatus.Ok, "Detail1 text");
                writeEngine.AddDetail(ProtocolStatus.Error, "Detail2 text");
                writeEngine.AddLinkedObject(new LinkedObject() {ObjectName = "EntityNameXYZ", ObjectId = "EntityId1"});
            }
            using (writeEngine.CreateAutoStartStop("Test protocol 2"))
            {
                writeEngine.AddDetail(ProtocolStatus.Ok, "Detail1 text");
                writeEngine.AddDetail(ProtocolStatus.Warning, "Detail2 text");
                writeEngine.AddLinkedObject(new LinkedObject() { ObjectName = "EntityNameXYZ", ObjectId = "EntityId2" });
            }
        }

        private static IProtocolWriteEngine<long> ProtocolWriteEngineWithRepositoryFile()
        {
            string dt = DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss-ffffff");
            var writeEngine = new ProtocolWriteEngine<long>(new DateTimeDefaultImpl(), new LoginNullImpl(), new ProtocolWriteRepositoryFile(
                "D:\\protocol " + dt + " HeaderId{0}.txt"));
            return writeEngine;
        }
    }
}
