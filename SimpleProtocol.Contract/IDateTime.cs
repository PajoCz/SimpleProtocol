using System;

namespace SimpleProtocol.Contract
{
    public interface IDateTime
    {
        DateTime Now { get; }
    }
}