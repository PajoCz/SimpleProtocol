using System;
using System.IO;
using SimpleProtocol.Contract;
using SimpleProtocol.Contract.Write;
using SimpleProtocol.Engine.Extensions;

namespace SimpleProtocol.Engine.Repository
{

    public class ProtocolWriteRepositoryFile : IProtocolWriteRepository<long>
    {
        private readonly string _FileNamePattern;
        private readonly Random _Random = new Random();

        private string FileNamePatternUsed(long p_HeaderId)
        {
            return string.Format(_FileNamePattern, p_HeaderId);
        }

        public ProtocolWriteRepositoryFile(string p_FileNamePattern)
        {
            _FileNamePattern = p_FileNamePattern;
        }

        public long Start(DateTime p_DateTimeNow, string p_Login, string p_HeaderName)
        {
            var headerId = _Random.NextLong();
            SaveLineWithDateTimePrefix(headerId, $"BEGIN [login={p_Login}]: Name='{p_HeaderName}'", p_DateTimeNow);
            return headerId;
        }

        //public long StartUniqueLinkedObject(DateTime p_DateTimeNow, string p_Login, string p_HeaderName, LinkedObject p_LinkedObject)
        //{
        //    throw new NotImplementedException();
        //}

        public void AddDetail(long p_HeaderId, DateTime p_DateTimeNow, ProtocolStatus p_Status, string p_Text)
        {
            SaveLineWithDateTimePrefix(p_HeaderId, $"Detail [{p_Status}]: {p_Text}", p_DateTimeNow);
        }

        public void Stop(long p_HeaderId, DateTime p_DateTimeNow)
        {
            SaveLineWithDateTimePrefix(p_HeaderId, "END", p_DateTimeNow);
        }

        public void AddLinkedObject(long p_HeaderId, LinkedObject p_LinkedObject)
        {
            if (p_LinkedObject == null) throw new ArgumentNullException(nameof(p_LinkedObject));

            SaveLine(p_HeaderId, $"ADD LinkedObject Name='{p_LinkedObject.ObjectName}' Id='{p_LinkedObject.ObjectId}'");
        }

        private void SaveLine(long p_HeaderId, string p_Text, string p_PrefixString = null)
        {
            File.AppendAllText(FileNamePatternUsed(p_HeaderId), $"{p_PrefixString}{p_Text}{Environment.NewLine}");
        }

        private void SaveLineWithDateTimePrefix(long p_HeaderId, string p_Text, DateTime p_DateTimeNow)
        {
            SaveLine(p_HeaderId, p_Text, $"{p_DateTimeNow:yyyy-MM-dd HH:mm:ss.ffffff} : ");
        }
    }
}