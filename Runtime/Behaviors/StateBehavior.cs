using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace SLS.StateMachineH 
{

    /// <summary>  
    /// Behavior Scripts attached to a <see cref="StateMachineH.State"/>. Inherit from this to create functionality.  
    /// </summary>  
    [RequireComponent(typeof(State))]
    public abstract class StateBehavior : MonoBehaviour
    {
        /// <summary>  
        /// The <see cref="StateMachine"/> owning this behavior. Likely the most important field you'll be referencing a lot.  
        /// Override with the "new" keyword with an expression like "=> M as MyStateMachine" to get a custom <see cref="StateMachine"/>.  
        /// </summary>  
        [field: SerializeField, HideInInspector] public StateMachine Machine { get; internal set; }

        /// <summary>  
        /// The current <see cref="StateMachineH.State"/>. Useful for referencing this SubObject.  
        /// </summary>  
        [field: SerializeField, HideInInspector] public State State { get; internal set; }

        /// <summary>  
        /// An indirection to access the <see cref="StateMachine"/>'s <see cref="GameObject"/> property.  
        /// </summary>  
        public new GameObject gameObject => Machine.gameObject;

        /// <summary>  
        /// An indirection to access the <see cref="StateMachine"/>'s <see cref="Transform"/> property.  
        /// </summary>  
        public new Transform transform => Machine.transform;

        /// <summary>  
        /// Sets up the <see cref="StateBehavior"/> and its serialized references with the specified <see cref="State"/> and marks it dirty if required.  
        /// </summary>  
        /// <param name="state">The <see cref="State"/> to associate with this behavior.</param>  
        /// <param name="makeDirty">Whether to mark the behavior as dirty in the editor.</param>  
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

        /// <summary>  
        /// Called during the setup process. Override to add custom setup logic.  
        /// </summary>  
        internal virtual void OnSetup() { }

        /// <summary>  
        /// Resets the <see cref="StateBehavior"/> to its default state.  
        /// Ensures the <see cref="State"/> and <see cref="StateMachine"/> references are properly initialized.  
        /// </summary>  
        internal void Reset()
        {
            if (State == null) State = GetComponent<State>();
            if (State != null) Machine = State.Machine;
        }

        /// <summary>  
        /// Called during the Awake phase. Override to add custom logic.  
        /// </summary>  
        internal virtual void OnAwake() { }

        /// <summary>  
        /// Called during the Update phase. Override to add custom logic.  
        /// </summary>  
        internal virtual void OnUpdate() { }

        /// <summary>  
        /// Called during the FixedUpdate phase. Override to add custom logic.  
        /// </summary>  
        internal virtual void OnFixedUpdate() { }

        /// <summary>  
        /// Called when entering the <see cref="State"/>. Override to add custom logic.  
        /// </summary>  
        /// <param name="prev">The previous <see cref="State"/>.</param>  
        /// <param name="isFinal">Indicates if this is the final <see cref="State"/>.</param>  
        internal virtual void OnEnter(State prev, bool isFinal) { }

        /// <summary>  
        /// Called when exiting the <see cref="State"/>. Override to add custom logic.  
        /// </summary>  
        /// <param name="next">The next <see cref="State"/>.</param>  
        internal virtual void OnExit(State next) { }

        /// <summary>  
        /// Retrieves a <see cref="Component"/> from the associated <see cref="StateMachine"/>.  
        /// </summary>  
        /// <typeparam name="C">The type of <see cref="Component"/> to retrieve.</typeparam>  
        /// <returns>The <see cref="Component"/> of type <typeparamref name="C"/>.</returns>  
        public C GetComponentFromMachine<C>() where C : Component => Machine.GetComponent<C>();

        /// <summary>  
        /// Attempts to retrieve a <see cref="Component"/> from the associated <see cref="StateMachine"/>.  
        /// </summary>  
        /// <typeparam name="C">The type of <see cref="Component"/> to retrieve.</typeparam>  
        /// <param name="result">The retrieved <see cref="Component"/>, if found.</param>  
        /// <returns>True if the <see cref="Component"/> was found; otherwise, false.</returns>  
        public bool TryGetComponentFromMachine<C>(out C result) where C : Component => Machine.TryGetComponent(out result);

        /// <summary>  
        /// Gets whether the <see cref="StateMachineH.State"> is currently active. 
        /// </summary>  
        public static implicit operator bool(StateBehavior B) => B != null && B.State.Active;
    }
}