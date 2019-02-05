using System;
using SimpleProtocol.Contract;
using SimpleProtocol.Contract.Extensions;
using SimpleProtocol.Contract.Write;

namespace SimpleProtocol.Engine.Write
{
    /// <summary>
    ///     Has InnerState - must be unique instance for every call / thread
    /// </summary>
    public class ProtocolWriteHeader<THeaderId, TDetailId> : IProtocolWriteHeader<THeaderId, TDetailId>, IDisposable
    {
        #region Injected dependencies

        private readonly IDateTime _DateTime;
        private readonly ILogin _Login;
        private readonly IProtocolWriteRepository<THeaderId, TDetailId> _ProtocolWriteRepository;
        private readonly bool _AutoStop;

        #endregion

        public ProtocolWriteHeader(IDateTime p_DateTime, ILogin p_Login, IProtocolWriteRepository<THeaderId, TDetailId> p_ProtocolWriteRepository, bool p_AutoStop)
        {
            _DateTime = p_DateTime;
            _Login = p_Login;
            _ProtocolWriteRepository = p_ProtocolWriteRepository;
            _AutoStop = p_AutoStop;
        }

        #region Object states

        public THeaderId HeaderId { get; private set; }
        public ProtocolWriteHeaderInnerState InnerState { get; private set; }
        public string Login { get; set; }

        #endregion

        #region Write methods

        public THeaderId Start(string p_HeaderName, LinkedObject p_LinkedObject = null)
        {
            if (InnerState == ProtocolWriteHeaderInnerState.Started)
                throw new ProtocolWriteHeaderInnerStateException(
                    $"InnerState is {InnerState}, but must be {ProtocolWriteHeaderInnerState.Created} / {ProtocolWriteHeaderInnerState.Stopped}");
            InnerState = ProtocolWriteHeaderInnerState.Started;
            StartedTime = _DateTime.Now;
            HeaderId = _ProtocolWriteRepository.Start(StartedTime.Value, _Login.Login, p_HeaderName);
            if (p_LinkedObject != null)
            {
                _ProtocolWriteRepository.AddLinkedObject(HeaderId, p_LinkedObject);
            }
            return HeaderId;
        }

        public DateTime? StartedTime { get; private set; }

        //public THeaderId StartUniqueLinkedObject(string p_HeaderName, LinkedObject p_LinkedObject)
        //{
        //    if (InnerState == ProtocolWriteHeaderInnerState.Started)
        //        throw new ProtocolWriteHeaderInnerStateException(
        //            $"InnerState is {InnerState}, but must be {ProtocolWriteHeaderInnerState.Created} / {ProtocolWriteHeaderInnerState.Stopped}");
        //    InnerState = ProtocolWriteHeaderInnerState.Started;
        //    HeaderId = _ProtocolWriteRepository.StartUniqueLinkedObject(_DateTime.Now, _Login.Login, p_HeaderName, p_LinkedObject);
        //    return HeaderId;
        //}

        public TDetailId AddDetail(ProtocolStatus p_Status, string p_Text)
        {
            if (InnerState != ProtocolWriteHeaderInnerState.Started)
                throw new ProtocolWriteHeaderInnerStateException($"InnerState is {InnerState}, but must be {ProtocolWriteHeaderInnerState.Started}");
            var result = _ProtocolWriteRepository.AddDetail(HeaderId, _DateTime.Now, p_Status, p_Text);
            if (p_Status != ProtocolStatus.EndProcess 
                && (WorstAddedDetailStatus == null || ProtocolStatusExtensions.StatusWorstIndex[WorstAddedDetailStatus.Value] < ProtocolStatusExtensions.StatusWorstIndex[p_Status]))
                WorstAddedDetailStatus = p_Status;
            return result;
        }

        public ProtocolStatus? WorstAddedDetailStatus { get; private set; }

        public void AddLinkedObject(LinkedObject p_LinkedObject)
        {
            if (InnerState != ProtocolWriteHeaderInnerState.Started)
                throw new ProtocolWriteHeaderInnerStateException($"InnerState is {InnerState}, but must be {ProtocolWriteHeaderInnerState.Started}");
            _ProtocolWriteRepository.AddLinkedObject(HeaderId, p_LinkedObject);
        }

        public void Stop()
        {
            if (InnerState != ProtocolWriteHeaderInnerState.Started)
                throw new ProtocolWriteHeaderInnerStateException($"InnerState is {InnerState}, but must be {ProtocolWriteHeaderInnerState.Started}");
            InnerState = ProtocolWriteHeaderInnerState.Stopped;
            StoppedTime = _DateTime.Now;
            _ProtocolWriteRepository.Stop(HeaderId, StoppedTime.Value);
        }

        public DateTime? StoppedTime { get; private set; }

        #endregion

        public void Dispose()
        {
            if (_AutoStop && InnerState == ProtocolWriteHeaderInnerState.Started)
            {
                Stop();
            }
        }
    }
}