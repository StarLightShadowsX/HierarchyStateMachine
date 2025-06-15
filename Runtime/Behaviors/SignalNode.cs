using UnityEngine;

#if ULT_EVENTS
using EVENT = UltEvents.UltEvent;
#else
using EVENT = UnityEngine.Events.UnityEvent;
#endif


namespace SLS.StateMachineH
{
    [RequireComponent(typeof(State))]
    public class SignalNode : StateBehavior
    {
        public SignalSet signals = new();
        [SerializeField] private bool lockOnEnter = false;

        public bool Locked { get; private set; }

        public EVENT this[string name] => signals[name];

        public bool FireSignal(Signal signal)
        {
            if (signals.ContainsKey(signal.name) && (!Locked || signal.ignoreLock))
            {
                signals[signal.name]?.Invoke();
                return true;
            }
            else return false;
        }

        public void Unlock() => Locked = false;
        public void Lock() => Locked = true;

        internal override void OnEnter(State prev, bool isFinal)
        {
            if (lockOnEnter) Locked = true;
        }
    }
}
