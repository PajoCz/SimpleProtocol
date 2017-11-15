using System;
using SimpleProtocol.Contract;
using SimpleProtocol.Contract.Write;

namespace SimpleProtocol.Engine.Write
{
    /// <summary>
    ///     Has InnerState - must be unique instance for every call / thread
    /// </summary>
    public class ProtocolWriteHeader<THeaderId> : IProtocolWriteHeader<THeaderId>, IDisposable
    {
        #region Injected dependencies

        private readonly IDateTime _DateTime;
        private readonly ILogin _Login;
        private readonly IProtocolWriteRepository<THeaderId> _ProtocolWriteRepository;
        private readonly bool _AutoStop;

        #endregion

        public ProtocolWriteHeader(IDateTime p_DateTime, ILogin p_Login, IProtocolWriteRepository<THeaderId> p_ProtocolWriteRepository, bool p_AutoStop)
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
            HeaderId = _ProtocolWriteRepository.Start(_DateTime.Now, _Login.Login, p_HeaderName);
            if (p_LinkedObject != null)
            {
                _ProtocolWriteRepository.AddLinkedObject(HeaderId, p_LinkedObject);
            }
            return HeaderId;
        }

        //public THeaderId StartUniqueLinkedObject(string p_HeaderName, LinkedObject p_LinkedObject)
        //{
        //    if (InnerState == ProtocolWriteHeaderInnerState.Started)
        //        throw new ProtocolWriteHeaderInnerStateException(
        //            $"InnerState is {InnerState}, but must be {ProtocolWriteHeaderInnerState.Created} / {ProtocolWriteHeaderInnerState.Stopped}");
        //    InnerState = ProtocolWriteHeaderInnerState.Started;
        //    HeaderId = _ProtocolWriteRepository.StartUniqueLinkedObject(_DateTime.Now, _Login.Login, p_HeaderName, p_LinkedObject);
        //    return HeaderId;
        //}

        public void AddDetail(ProtocolStatus p_Status, string p_Text)
        {
            if (InnerState != ProtocolWriteHeaderInnerState.Started)
                throw new ProtocolWriteHeaderInnerStateException($"InnerState is {InnerState}, but must be {ProtocolWriteHeaderInnerState.Started}");
            _ProtocolWriteRepository.AddDetail(HeaderId, _DateTime.Now, p_Status, p_Text);
        }

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
            _ProtocolWriteRepository.Stop(HeaderId, _DateTime.Now);
        }

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