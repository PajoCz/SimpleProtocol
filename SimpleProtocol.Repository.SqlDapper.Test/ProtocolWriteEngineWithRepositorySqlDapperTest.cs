using System.Configuration;
using NUnit.Framework;
using SimpleProtocol.Contract;
using SimpleProtocol.Contract.Write;
using SimpleProtocol.Engine;
using SimpleProtocol.Engine.Write;

namespace SimpleProtocol.Repository.SqlDapper.Test
{
    [TestFixture]
    public class ProtocolWriteEngineWithRepositorySqlDapperTest
    {
        [Test]
        public void Test()
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
    }
}