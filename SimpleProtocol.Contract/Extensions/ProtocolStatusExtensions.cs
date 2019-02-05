using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace SimpleProtocol.Contract.Extensions
{
    public static class ProtocolStatusExtensions
    {
        public static ProtocolStatus Worst(this ProtocolStatus p_ProtocolStatus, ProtocolStatus p_ProtocolStatus2)
        {
            return StatusWorstIndex[p_ProtocolStatus] > StatusWorstIndex[p_ProtocolStatus2]
                ? p_ProtocolStatus
                : p_ProtocolStatus2;
        }

        public static ReadOnlyDictionary<ProtocolStatus, int> StatusWorstIndex =
            new ReadOnlyDictionary<ProtocolStatus, int>(new Dictionary<ProtocolStatus, int>()
            {
                {ProtocolStatus.Info, 1},
                {ProtocolStatus.Ok, 2},
                {ProtocolStatus.Warning, 3},
                {ProtocolStatus.Error, 4},
                {ProtocolStatus.Failed, 5},
                {ProtocolStatus.EndProcess, 6},
            });
    }
}