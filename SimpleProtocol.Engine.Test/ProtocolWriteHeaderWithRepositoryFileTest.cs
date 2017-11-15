using System;
using NUnit.Framework;
using SimpleProtocol.Contract;
using SimpleProtocol.Contract.Write;
using SimpleProtocol.Engine.Repository;
using SimpleProtocol.Engine.Write;

namespace SimpleProtocol.Engine.Test
{
    [TestFixture]
    public class ProtocolWriteHeaderWithRepositoryFileTest
    {
        [Test]
        public void Start_AddDetail_Stop()
        {
            var writeHeaderFactory = ProtocolWriteHeaderFactoryWithRepositoryFile();
            using (var writeHeader = writeHeaderFactory.Create("Test protocol 1"))
            {
                writeHeader.AddDetail(ProtocolStatus.Ok, "Detail1 text");
                writeHeader.AddDetail(ProtocolStatus.Error, "Detail2 text");
                writeHeader.AddLinkedObject(new LinkedObject() {ObjectName = "EntityNameXYZ", ObjectId = "EntityId1"});
            }
            using (var writeHeader = writeHeaderFactory.Create("Test protocol 2"))
            {
                writeHeader.AddDetail(ProtocolStatus.Ok, "Detail1 text");
                writeHeader.AddDetail(ProtocolStatus.Warning, "Detail2 text");
                writeHeader.AddLinkedObject(new LinkedObject() { ObjectName = "EntityNameXYZ", ObjectId = "EntityId2" });
            }
        }

        private static ProtocolWriteHeaderFactory<long> ProtocolWriteHeaderFactoryWithRepositoryFile()
        {
            string dt = DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss-ffffff");
            return new ProtocolWriteHeaderFactory<long>(new DateTimeDefaultImpl(), new LoginNullImpl(), new ProtocolWriteRepositoryFile(
                "D:\\protocol " + dt + " HeaderId{0}.txt"));
        }
    }
}
