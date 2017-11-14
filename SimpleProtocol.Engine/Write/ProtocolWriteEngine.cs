using System;
using SimpleProtocol.Contract;
using SimpleProtocol.Contract.Write;

namespace SimpleProtocol.Engine.Write
{
    /// <summary>
    ///     Has InnerState - must be unique instance for every call / thread
    ///     For more comfort using Start / Stop may be used ProtocolWriteEngineAutoStartStop
    /// </summary>
    public class ProtocolWriteEngine<THeaderId> : IProtocolWriteEngine<THeaderId>
    {
        #region Injected dependencies

        private readonly IDateTime _DateTime;
        private readonly ILogin _Login;
        private readonly IProtocolWriteRepository<THeaderId> _ProtocolWriteRepository;

        #endregion

        public ProtocolWriteEngine(IDateTime p_DateTime, ILogin p_Login, IProtocolWriteRepository<THeaderId> p_ProtocolWriteRepository)
        {
            _DateTime = p_DateTime;
            _Login = p_Login;
            _ProtocolWriteRepository = p_ProtocolWriteRepository;
        }

        #region Object states

        public THeaderId HeaderId { get; private set; }
        public ProtocolWriteEngineInnerState InnerState { get; private set; }
        public string Login { get; set; }

        #endregion

        public IProtocolWriteEngineAutoStartStop CreateAutoStartStop(string p_HeaderName)
        {
            return new ProtocolWriteEngineAutoStartStop<THeaderId>(this, p_HeaderName);
        }

        #region Write methods

        public THeaderId Start(string p_HeaderName, LinkedObject p_LinkedObject = null)
        {
            if (InnerState == ProtocolWriteEngineInnerState.Started)
                throw new ProtocolWriteEngineInnerStateException(
                    $"InnerState is {InnerState}, but must be {ProtocolWriteEngineInnerState.Created} / {ProtocolWriteEngineInnerState.Stopped}");
            InnerState = ProtocolWriteEngineInnerState.Started;
            HeaderId = _ProtocolWriteRepository.Start(_DateTime.Now, _Login.Login, p_HeaderName);
            if (p_LinkedObject != null)
            {
                _ProtocolWriteRepository.AddLinkedObject(HeaderId, p_LinkedObject);
            }
            return HeaderId;
        }

        public THeaderId StartUniqueLinkedObject(string p_HeaderName, LinkedObject p_LinkedObject)
        {
            if (InnerState == ProtocolWriteEngineInnerState.Started)
                throw new ProtocolWriteEngineInnerStateException(
                    $"InnerState is {InnerState}, but must be {ProtocolWriteEngineInnerState.Created} / {ProtocolWriteEngineInnerState.Stopped}");
            InnerState = ProtocolWriteEngineInnerState.Started;
            HeaderId = _ProtocolWriteRepository.StartUniqueLinkedObject(_DateTime.Now, _Login.Login, p_HeaderName, p_LinkedObject);
            return HeaderId;
        }

        public void AddDetail(ProtocolStatus p_Status, string p_Text)
        {
            if (InnerState != ProtocolWriteEngineInnerState.Started)
                throw new ProtocolWriteEngineInnerStateException($"InnerState is {InnerState}, but must be {ProtocolWriteEngineInnerState.Started}");
            _ProtocolWriteRepository.AddDetail(HeaderId, _DateTime.Now, p_Status, p_Text);
        }

        public void AddLinkedObject(LinkedObject p_LinkedObject)
        {
            if (InnerState != ProtocolWriteEngineInnerState.Started)
                throw new ProtocolWriteEngineInnerStateException($"InnerState is {InnerState}, but must be {ProtocolWriteEngineInnerState.Started}");
            _ProtocolWriteRepository.AddLinkedObject(HeaderId, p_LinkedObject);
        }

        public void Stop()
        {
            if (InnerState != ProtocolWriteEngineInnerState.Started)
                throw new ProtocolWriteEngineInnerStateException($"InnerState is {InnerState}, but must be {ProtocolWriteEngineInnerState.Started}");
            InnerState = ProtocolWriteEngineInnerState.Stopped;
            _ProtocolWriteRepository.Stop(HeaderId, _DateTime.Now);
        }

        #endregion
    }
}