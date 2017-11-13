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
    public class ProtocolWriteEngineWithRepositorySqlDapperTest
    {
        [Test]
        public void WriteEngine_CreateAutoStartStop_MoreAddDetails_MoreAddLinkedObject()
        {
            var engine = new ProtocolWriteEngine(new DateTimeDefaultImpl(),
                new ProtocolWriteRepositorySqlDapper(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString));
            using (engine.CreateAutoStartStop(new HeaderEntityWrite {HeaderName = "HeaderName1", Login = "Login1"}))
            {
                engine.AddDetail(new DetailEntityWrite {Status = ProtocolStatus.Ok, Text = "Detail text 1"});
                engine.AddDetail(new DetailEntityWrite {Status = ProtocolStatus.Warning, Text = "Detail text 2"});
                engine.AddLinkedObject(new LinkedObject {ObjectName = "ObjectNameOfTypeInt", ObjectId = 12});
                engine.AddLinkedObject(new LinkedObject {ObjectName = "ObjectNameOfTypeString", ObjectId = "12a"});
            }
        }

        [Test]
        public void WriteEngine_StartUniqueLinkedObject_FirstCallCreateHeader_SecondCallAddDetailsToSameHeader_CheckDataByReadEngine()
        {
            var linkedObject = new LinkedObject() {ObjectName = "ObjectName1", ObjectId = "ObjectId" + new Random().Next()};

            var writeEngine = new ProtocolWriteEngine(new DateTimeDefaultImpl(),
                new ProtocolWriteRepositorySqlDapper(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString));
            writeEngine.StartUniqueLinkedObject(new HeaderEntityWrite() { HeaderName = "HeaderForObject1", Login = "CreatedFromLoginX"}, linkedObject);
            writeEngine.AddDetail(new DetailEntityWrite { Status = ProtocolStatus.Ok, Text = "Detail text 1" });
            writeEngine.Stop();

            writeEngine.StartUniqueLinkedObject(new HeaderEntityWrite() { HeaderName = "HeaderForObject1NextCall", Login = "CreatedFromLoginXNextCall" }, linkedObject);
            writeEngine.AddDetail(new DetailEntityWrite { Status = ProtocolStatus.Ok, Text = "Detail text 2" });
            writeEngine.Stop();

            var readEngine = new ProtocolReadEngine(new ProtocolReadRepositorySqlDapper(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString));
            var found = readEngine.FindByLinkedObject(linkedObject, true).ToList();

            //ASSERT
            Assert.AreEqual(1, found.Count);
            Assert.AreEqual("HeaderForObject1", found.First().Name);
            Assert.AreEqual("CreatedFromLoginX", found.First().CreatedLogin);
            Assert.AreEqual(4, found.First().Details.Count());
            
            //check detail properties
            Assert.AreEqual(ProtocolStatus.Ok, found.First().Details.ToList()[0].Status);
            Assert.AreEqual(ProtocolStatus.EndProcess, found.First().Details.ToList()[1].Status);
            Assert.AreEqual(ProtocolStatus.Ok, found.First().Details.ToList()[2].Status);
            Assert.AreEqual(ProtocolStatus.EndProcess, found.First().Details.ToList()[3].Status);

            Assert.AreEqual("Detail text 1", found.First().Details.ToList()[0].Text);
            Assert.AreEqual(null, found.First().Details.ToList()[1].Text);
            Assert.AreEqual("Detail text 2", found.First().Details.ToList()[2].Text);
            Assert.AreEqual(null, found.First().Details.ToList()[3].Text);
        }
    }
}