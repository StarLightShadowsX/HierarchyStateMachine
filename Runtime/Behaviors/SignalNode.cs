﻿using UnityEngine;

#if ULT_EVENTS
using EVENT = UltEvents.UltEvent;
#else
using EVENT = UnityEngine.Events.UnityEvent;
#endif


namespace SLS.StateMachineH
{
    /// <summary>  
    /// Represents a node in the <see cref="StateMachine"/> that can recieve signals and enact events.  
    /// </summary>  
    [RequireComponent(typeof(State))]
    public class SignalNode : StateBehavior
    {
        /// <summary>  
        /// The signals associated with this node.  
        /// </summary>  
        public SignalSet signals = new();

        /// <summary>  
        /// Indicates whether the node should lock itself upon entering a state.  
        /// </summary>  
        [SerializeField] private bool lockOnEnter = false;

        /// <summary>  
        /// The value indicating whether the node is currently locked.  
        /// </summary>  
        public bool Locked { get; private set; }

        /// <summary>  
        /// Indexer to access events associated with a signal name.  
        /// </summary>  
        /// <param name="name">The name of the signal.</param>  
        /// <returns>The event associated with the signal name.</returns>  
        public EVENT this[string name] => signals[name];

        /// <summary>  
        /// Fires a signal if it exists and meets the conditions for invocation.  
        /// </summary>  
        /// <param name="signal">The signal to fire.</param>  
        /// <returns>True if the signal was fired; otherwise, false.</returns>  
        public bool FireSignal(Signal signal)
        {
            if (signals.ContainsKey(signal.name) && (!Locked || signal.ignoreLock))
            {
                signals[signal.name]?.Invoke();
                return true;
            }
            else return false;
        }

        /// <summary>  
        /// Unlocks the node, allowing signals to be fired.  
        /// </summary>  
        public void Unlock() => Locked = false;

        /// <summary>  
        /// Locks the node, preventing signals from being fired.  
        /// </summary>  
        public void Lock() => Locked = true;

        /// <summary>  
        /// Called when entering a state. Locks the node if <see cref="lockOnEnter"/> is true.  
        /// </summary>  
        /// <param name="prev">The previous state.</param>  
        /// <param name="isFinal">Indicates whether this is the final state.</param>  
        protected override void OnEnter(State prev, bool isFinal)
        {
            if (lockOnEnter) Locked = true;
        }
        /// <summary>
        /// Called when exiting a state. Unlocks the node to ensure no stale lockage.
        /// </summary>
        /// <param name="next"></param>
        protected override void OnExit(State next) => Locked = false;

    }
}
