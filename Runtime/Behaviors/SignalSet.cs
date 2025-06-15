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
    [Serializable]
    public class SignalSet : SerializedDictionary<string, EVENT> { }

    public class Signal
    {
        public Signal(string name, float queueTime = .5f, bool ignoreLock = false, bool allowDuplicates = false)
        {
            this.name = name;
            this.queueTime = queueTime;
            this.ignoreLock = ignoreLock;
            this.allowDuplicates = allowDuplicates;
        }
        public string name;
        public float queueTime = .5f;
        public bool ignoreLock = false;
        public bool allowDuplicates = false;
        public static implicit operator string(Signal signal) => signal.name;
        public static implicit operator Signal(string name) => new(name);
    }

}
