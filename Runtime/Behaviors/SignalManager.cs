using System;
using System.Collections.Generic;
using UnityEngine;

#if ULT_EVENTS
using EVENT = UltEvents.UltEvent;
#else
using EVENT = UnityEngine.Events.UnityEvent;
#endif

namespace SLS.StateMachineH
{
    [RequireComponent(typeof(StateMachine))]
    public class SignalManager : StateBehavior
    {
        public SignalSet globalSignals = new();

        public bool queueSignals = true;

        public EVENT this[string name] => globalSignals[name];
        public SignalNode GetCurrentNode() => Machine.CurrentState.GetComponent<SignalNode>();
        public bool TryCurrentNode(out SignalNode signalNode) => Machine.CurrentState.TryGetComponent(out signalNode);


        public bool FireSignal(Signal signal)
        {
            bool signalFired = false;
            if (TryCurrentNode(out SignalNode signalNode) && signalNode.FireSignal(signal.name)) signalFired = true;
            else if (globalSignals.ContainsKey(signal.name))
            {
                globalSignals[signal]?.Invoke();
                signalFired = true;
            }

            if (signalFired)
            {
                if(queueSignals && SignalQueue.Count > 0)
                    FireSignal(SignalQueue.Dequeue());
            }
            else if (queueSignals && signal.queueTime > 0)
            {
                if (signal.allowDuplicates || SignalQueue.Count == 0 || SignalQueue.Peek().name != signal.name)
                {
                    SignalQueue.Enqueue(signal);
                    ActiveSignalLength = signal.queueTime;
                    SignalQueueTimer = ActiveSignalLength;
                }
            }

            return signalFired;
        }
        public void Lock()
        { if (TryCurrentNode(out SignalNode signalNode)) signalNode.Lock(); }
        public void Unlock()
        {
            if (TryCurrentNode(out SignalNode signalNode))
            {
                signalNode.Unlock();
                if (queueSignals && SignalQueue.Count > 0) FireSignal(SignalQueue.Dequeue());
            }
        }

        public Queue<Signal> SignalQueue { get; private set; } = new();
        public float ActiveSignalLength { get; private set; } = 0f;
        public float SignalQueueTimer { get; private set; } = 0f;

        public override void OnUpdate()
        {
            if(queueSignals && ActiveSignalLength > 0f)
            {
                SignalQueueTimer -= Time.deltaTime;
                if(SignalQueueTimer <= 0f) FireSignal(SignalQueue.Dequeue());
            }
        }

    }
}
