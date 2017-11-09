using System;
using SimpleProtocol.Contract;

namespace SimpleProtocol.Engine
{
    public class DateTimeDefaultImpl : IDateTime
    {
        public DateTime Now => DateTime.Now;
    }
}