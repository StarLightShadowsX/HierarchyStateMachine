using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#if ULT_EVENTS
using EVENT = UltEvents.UltEvent;
#else
using EVENT = UnityEngine.Events.UnityEvent;
#endif

using SLS.StateMachineH.SerializedDictionary;

namespace SLS.StateMachineH
{
    /// <summary>  
    /// Represents a dictionary of signals, where each signal is associated with a unique string key.  
    /// </summary>  
    [Serializable]
    public class SignalSet : SerializedDictionary<string, EVENT> { }

    /// <summary>  
    /// Represents a signal with properties for queue time, lock behavior, and duplicate allowance.  
    /// </summary>  
    public class Signal
    {
        /// <summary>  
        /// Initializes a new instance of the <see cref="Signal"/> class.  
        /// </summary>  
        /// <param name="name">The name of the signal.</param>  
        /// <param name="queueTime">The time the signal should remain in the queue. Default is 0.5 seconds.</param>  
        /// <param name="ignoreLock">Indicates whether the signal should ignore lock conditions.</param>  
        /// <param name="allowDuplicates">Indicates whether duplicate signals are allowed.</param>  
        public Signal(string name, float queueTime = .5f, bool ignoreLock = false, bool allowDuplicates = false)
        {
            this.name = name;
            this.queueTime = queueTime;
            this.ignoreLock = ignoreLock;
            this.allowDuplicates = allowDuplicates;
        }

        /// <summary>  
        /// Gets or sets the name of the signal.  
        /// </summary>  
        public string name;

        /// <summary>  
        /// The time the signal should remain in the queue.  
        /// </summary>  
        public float queueTime = .5f;

        /// <summary>  
        /// A value indicating whether the signal should ignore lock conditions.  
        /// </summary>  
        public bool ignoreLock = false;

        /// <summary>  
        /// A value indicating whether duplicate signals are allowed.  
        /// </summary>  
        public bool allowDuplicates = false;

        /// <summary>  
        /// Implicitly converts a <see cref="Signal"/> instance to its name as a string.  
        /// </summary>  
        /// <param name="signal">The signal to convert.</param>  
        /// <returns>The name of the signal.</returns>  
        public static implicit operator string(Signal signal) => signal.name;

        /// <summary>  
        /// Implicitly converts a string to a <see cref="Signal"/> instance.  
        /// </summary>  
        /// <param name="name">The name of the signal.</param>  
        /// <returns>A new <see cref="Signal"/> instance with the specified name.</returns>  
        public static implicit operator Signal(string name) => new(name);
    }

}
