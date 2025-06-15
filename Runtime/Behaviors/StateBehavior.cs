using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace SLS.StateMachineH {

    /// <summary>
    /// Behavior Scripts attached to a state. Inherit from this to create functionality.
    /// </summary>
    [RequireComponent(typeof(State))]
    public abstract class StateBehavior : MonoBehaviour
    {

        /// <summary>
        /// The State Machine owning this behavior. Likely the most important field you'll be referencing a lot.<br />
        /// Override with the "new" keyword with an expression like "=> M as MyStateMachine" to get a custom StateMachine
        /// </summary>
        [field: SerializeField, HideInInspector] public StateMachine Machine { get; internal set; }
        /// <summary>
        /// The current State. Usefull for referencing this SubObject.
        /// </summary>
        [field: SerializeField, HideInInspector] public State State { get; internal set; }
        /// <summary>
        /// An indirection to access the State Machine's gameObject property.
        /// </summary>
        public new GameObject gameObject => Machine.gameObject;
        /// <summary>
        /// An indirection to access the State Machine's transform property.
        /// </summary>
        public new Transform transform => Machine.transform;



        public void Setup(State @state, bool makeDirty = false)
        {
            this.State = @state;
            Machine = State != null
                ? @state.Machine
                : GetComponent<StateMachine>();

            this.OnSetup();

#if UNITY_EDITOR
            if (makeDirty) EditorUtility.SetDirty(this);
#endif
        }
        internal virtual void OnSetup() { }

        internal void Reset()
        {
            if(State == null) State = GetComponent<State>();
            if (State != null) Machine = State.Machine;
        }

        internal virtual void OnAwake() { }
        internal virtual void OnUpdate() { }
        internal virtual void OnFixedUpdate() { }
        internal virtual void OnEnter(State prev, bool isFinal) { }
        internal virtual void OnExit(State next) { }

        public C GetComponentFromMachine<C>() where C : Component => Machine.GetComponent<C>();
        public bool TryGetComponentFromMachine<C>(out C result) where C : Component => Machine.TryGetComponent(out result);

        public static implicit operator bool(StateBehavior B) => B != null && B.State.Active;
    }
}