﻿using System;
using SimpleProtocol.Contract;
using SimpleProtocol.Contract.Write;

namespace SimpleProtocol.Engine.Write
{
    /// <summary>
    ///     Has InnerState - must be unique instance for every call / thread
    ///     For more comfort using Start / Stop may be used ProtocolWriteEngineAutoStartStop
    /// </summary>
    public class ProtocolWriteEngine : IProtocolWriteEngine<long>
    {
        #region Injected dependencies

        private readonly IDateTime _DateTime;
        private readonly IProtocolWriteRepository<long> _ProtocolWriteRepository;

        #endregion

        public ProtocolWriteEngine(IDateTime p_DateTime, IProtocolWriteRepository<long> p_ProtocolWriteRepository)
        {
            _DateTime = p_DateTime;
            _ProtocolWriteRepository = p_ProtocolWriteRepository;
        }

        #region Object states

        public long HeaderId { get; private set; }
        public ProtocolWriteEngineInnerState InnerState { get; private set; }

        #endregion

        public IProtocolWriteEngineAutoStartStop CreateAutoStartStop(HeaderEntityWrite p_HeaderEntityWrite)
        {
            return new ProtocolWriteEngineAutoStartStop<long>(this, p_HeaderEntityWrite);
        }


        #region Write methods

        public long Start(HeaderEntityWrite p_HeaderEntityWrite)
        {
            if (InnerState == ProtocolWriteEngineInnerState.Started)
                throw new ProtocolWriteEngineInnerStateException(
                    $"InnerState is {InnerState}, but must be {ProtocolWriteEngineInnerState.Created} / {ProtocolWriteEngineInnerState.Stopped}");
            InnerState = ProtocolWriteEngineInnerState.Started;
            HeaderId = _ProtocolWriteRepository.Start(_DateTime.Now, p_HeaderEntityWrite);
            return HeaderId;
        }

        public long StartUniqueLinkedObject(HeaderEntityWrite p_HeaderEntityWrite, LinkedObject p_LinkedObject)
        {
            if (InnerState == ProtocolWriteEngineInnerState.Started)
                throw new ProtocolWriteEngineInnerStateException(
                    $"InnerState is {InnerState}, but must be {ProtocolWriteEngineInnerState.Created} / {ProtocolWriteEngineInnerState.Stopped}");
            InnerState = ProtocolWriteEngineInnerState.Started;
            HeaderId = _ProtocolWriteRepository.StartUniqueLinkedObject(_DateTime.Now, p_HeaderEntityWrite, p_LinkedObject);
            return HeaderId;
        }

        public void AddDetail(DetailEntityWrite p_DetailEntityWrite)
        {
            if (InnerState != ProtocolWriteEngineInnerState.Started)
                throw new ProtocolWriteEngineInnerStateException($"InnerState is {InnerState}, but must be {ProtocolWriteEngineInnerState.Started}");
            _ProtocolWriteRepository.AddDetail(HeaderId, _DateTime.Now, p_DetailEntityWrite);
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