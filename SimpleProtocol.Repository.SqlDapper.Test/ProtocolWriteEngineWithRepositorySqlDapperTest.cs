using System;
using System.Configuration;
using System.Linq;
using NUnit.Framework;
using SimpleProtocol.Contract;
using SimpleProtocol.Contract.Write;
using SimpleProtocol.Engine;
using SimpleProtocol.Engine.Read;
using SimpleProtocol.Engine.Write;

namespace SimpleProtocol.Repository.SqlDapper.Test
{
    [TestFixture]
    public class ProtocolWriteHeaderWithRepositorySqlDapperTest
    {
        [Test]
        public void WriteHeader_Create_MoreAddDetails_MoreAddLinkedObject()
        {
            var writeHeaderFactory = new ProtocolWriteHeaderFactory<long>(new DateTimeDefaultImpl(), new LoginNullImpl("CreatedFromLoginX"),
                new ProtocolWriteRepositorySqlDapper(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString));
            using (var writeHeader = writeHeaderFactory.Create("HeaderName1"))
            {
                writeHeader.AddDetail(ProtocolStatus.Ok, "Detail text 1");
                writeHeader.AddDetail(ProtocolStatus.Warning, "Detail text 2");
                writeHeader.AddLinkedObject(new LinkedObject {ObjectName = "ObjectNameOfTypeInt", ObjectId = 12});
                writeHeader.AddLinkedObject(new LinkedObject {ObjectName = "ObjectNameOfTypeString", ObjectId = "12a"});
            }
        }

        [Test]
        public void WriteHeader_CreateAutoStop_CheckDataByReadHeader()
        {
            var linkedObject = new LinkedObject() { ObjectName = "ObjectName1", ObjectId = "ObjectId" + new Random().Next() };

            var writeHeaderFactory = new ProtocolWriteHeaderFactory<long>(new DateTimeDefaultImpl(), new LoginNullImpl("CreatedFromLoginX"),
                new ProtocolWriteRepositorySqlDapper(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString));
            using (var writeHeader = writeHeaderFactory.CreateAutoStop("HeaderForObject1", linkedObject))
            {
                writeHeader.AddDetail(ProtocolStatus.Ok, "Detail text 1");
            }

            using (var writeHeader = writeHeaderFactory.CreateAutoStop("HeaderForObject1NextCall", linkedObject))
            {
                writeHeader.AddDetail(ProtocolStatus.Ok, "Detail text 2");
            }

            var readHeader = new ProtocolReadHeader(new ProtocolReadRepositorySqlDapper(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString));
            var found = readHeader.FindByLinkedObject(linkedObject, true).ToList();

            //ASSERT
            Assert.AreEqual(2, found.Count);

            //first header
            Assert.AreEqual("HeaderForObject1", found.First().Name);
            Assert.AreEqual("CreatedFromLoginX", found.First().CreatedLogin);
            Assert.AreEqual(2, found.First().Details.Count());
            Assert.AreEqual(ProtocolStatus.Ok, found.First().Details.ToList()[0].Status);
            Assert.AreEqual(ProtocolStatus.EndProcess, found.First().Details.ToList()[1].Status);
            Assert.AreEqual("Detail text 1", found.First().Details.ToList()[0].Text);
            Assert.AreEqual(null, found.First().Details.ToList()[1].Text);

            //seconde header
            Assert.AreEqual("HeaderForObject1NextCall", found.Last().Name);
            Assert.AreEqual("CreatedFromLoginX", found.Last().CreatedLogin);
            Assert.AreEqual(2, found.Last().Details.Count());
            Assert.AreEqual(ProtocolStatus.Ok, found.Last().Details.ToList()[0].Status);
            Assert.AreEqual(ProtocolStatus.EndProcess, found.Last().Details.ToList()[1].Status);
            Assert.AreEqual("Detail text 2", found.Last().Details.ToList()[0].Text);
            Assert.AreEqual(null, found.Last().Details.ToList()[1].Text);
        }
    }
}