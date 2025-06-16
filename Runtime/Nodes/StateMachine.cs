using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace SLS.StateMachineH
{
    public class StateMachine : State, IPrebuildable
    {
        [field: SerializeField, SerializeReference] public List<State> Children { get; internal set; }
        [field: SerializeField] public bool StatesSetup { get; internal set; }
        [field: SerializeField] public Transform StateHolder { get; internal set; }

        public override bool Active => enabled;
        public State CurrentState { get; internal set; }
        public System.Action waitforMachineInit;

        public override StateType Type => StateType.Machine;
        public override bool HasChildren => true;



        protected virtual void Update() => DoUpdate();
        protected virtual void FixedUpdate() => DoFixedUpdate();

        #region Initialization  

        protected virtual void Awake()
        {
            if (!StatesSetup) Setup(this, this, -1);
            OnAwake();
            DoAwake();

            for (int i = 0; i < Behaviors.Length; i++) Behaviors[i].OnEnter(null, false);
            CurrentChild = Children[0];
            TransitionState(this);

            waitforMachineInit?.Invoke();
        }

        private void Reset() => SetupBasics();

        public override void Setup(StateMachine machine, State parent, int layer, bool makeDirty = false)
        {
            if (StateHolder == null || Machine == null) SetupBasics();
            if (StateHolder.childCount == 0)
                throw new System.Exception("Stateless State Machines are not supported. If you need to use StateBehaviors on something with only one state, create a dummy state.");

            this.PreSetup();

            {
                ChildCount = StateHolder.childCount;
                Children = new();
                for (int i = 0; i < ChildCount; i++)
                {
                    Children.Add(StateHolder.GetChild(i).GetComponent<State>());
                    Children[i].Setup(machine, this, layer + 1);
                }
            }

            Behaviors = GetComponents<StateBehavior>();
            for (int i = 0; i < Behaviors.Length; i++) Behaviors[i].Setup(this);

            StatesSetup = true;

#if UNITY_EDITOR
            if (makeDirty) EditorUtility.SetDirty(this);
#endif
        }



        private void SetupBasics()
        {
            if (Machine == null) Machine = this;
            Transform tryRoot = transform.Find("States");
            if (tryRoot)
            {
                StateHolder = tryRoot;
#if UNITY_EDITOR
                EditorUtility.SetDirty(this);
#endif
            }
            else
            {
                var newRoot = new GameObject("States");
                newRoot.transform.SetParent(transform, false);
                StateHolder = newRoot.transform;
            }
        }

        protected virtual void PreSetup() { }
        protected virtual void OnAwake() { }

        #endregion

        internal virtual void TransitionState(State nextState)
        {
            if (!Application.isPlaying ||
                nextState == null ||
                nextState == CurrentState
               ) return;

            var cursorTarget = nextState;
            int targetDepth = nextState.Layer;

            if (CurrentState != null)
            {
                CurrentState.DoExit(nextState);
                State cursorStart = CurrentState.Parent;
                int startDepth = CurrentState.Layer - 1;

                while (startDepth != targetDepth || cursorStart.Parent != cursorTarget.Parent)
                {
                    bool startUp = startDepth >= targetDepth && startDepth > -1;
                    bool targetUp = startDepth <= targetDepth && targetDepth > -1;

                    if (startUp)
                    {
                        cursorStart.DoExit(nextState);
                        cursorStart = cursorStart.Parent;
                        startDepth--;
                    }
                    if (targetUp)
                    {
                        ExitStates.Push(cursorTarget);
                        cursorTarget = cursorTarget.Parent;
                        targetDepth--;
                    }
                }

                if (startDepth == -1) CurrentChild = ExitStates.Peek();

                cursorStart.DoExit(nextState);
            }

            if(cursorTarget) ExitStates.Push(cursorTarget); 


            while(ExitStates.Count > 0 || nextState.HasChildren)
            {
                nextState = ExitStates.Count > 0 
                    ? ExitStates.Pop() 
                    : nextState.Children[0];

                nextState.CurrentChild = ExitStates.Count > 0 
                    ? ExitStates.Peek() 
                    : nextState.HasChildren 
                        ? nextState.Children[0] 
                        : null;

                nextState.DoEnter(CurrentState);
            }
            CurrentState = nextState; 
        }
        private static Stack<State> ExitStates = new();

        public void Build() => Setup(this, this, -1, true);

        internal void MarkDirty()
        {
            StatesSetup = false;
#if UNITY_EDITOR
            EditorUtility.SetDirty(this);
#endif
        }
    }
}