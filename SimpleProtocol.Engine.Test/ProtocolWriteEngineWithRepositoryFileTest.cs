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
            using (writeEngine.CreateAutoStartStop(new HeaderEntityWrite() { Login = "login1", HeaderName = "Test protocol 1" }))
            {
                writeEngine.AddDetail(new DetailEntityWrite() {Status = ProtocolStatus.Ok, Text = "Detail1 text"});
                writeEngine.AddDetail(new DetailEntityWrite() { Status = ProtocolStatus.Error, Text = "Detail2 text" });
                writeEngine.AddLinkedObject(new LinkedObject() {ObjectName = "EntityNameXYZ", ObjectId = "EntityId1"});
            }
            using (writeEngine.CreateAutoStartStop(new HeaderEntityWrite() { Login = "login1", HeaderName = "Test protocol 2" }))
            {
                writeEngine.AddDetail(new DetailEntityWrite() { Status = ProtocolStatus.Ok, Text = "Detail1 text" });
                writeEngine.AddDetail(new DetailEntityWrite() { Status = ProtocolStatus.Warning, Text = "Detail2 text" });
                writeEngine.AddLinkedObject(new LinkedObject() { ObjectName = "EntityNameXYZ", ObjectId = "EntityId2" });
            }
        }

        private static IProtocolWriteEngine<long, long> ProtocolWriteEngineWithRepositoryFile()
        {
            string dt = DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss-ffffff");
            var writeEngine = new ProtocolWriteEngine(new DateTimeDefaultImpl(), new ProtocolWriteRepositoryFile(
                "D:\\protocol " + dt + " HeaderId{0}.txt"));
            return writeEngine;
        }
    }
}
