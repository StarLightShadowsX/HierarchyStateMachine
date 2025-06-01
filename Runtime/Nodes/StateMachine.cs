using EditorAttributes;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

namespace SLS.StateMachineH.V5
{
    public class StateMachine : MonoBehaviour, IStateGroup, IPrebuildable
    {
        public bool Active => enabled;
        public StateBehavior[] Behaviors => behaviors;
        [SerializeField] protected StateBehavior[] behaviors;
        public IStateChild[] ChildNodes => childNodes;
        [SerializeField] protected IStateChild[] childNodes;
        public int ChildCount => childCount;
        [SerializeField] protected int childCount;
        public IStateChild CurrentChild => currentChild;
        [SerializeField] protected IStateChild currentChild;
        public State CurrentState { get; protected set; }
        public bool StatesSetup { get => statesSetup; private set => statesSetup = value; }
        [SerializeField] protected bool statesSetup;
        public Transform StateHolder { get => stateHolder; private set => stateHolder = value; }
        [SerializeField] protected Transform stateHolder;
        public System.Action waitforMachineInit;






        protected virtual void Update() => DoUpdate();

        protected virtual void FixedUpdate() => DoFixedUpdate();




        #region Initialization

        protected virtual void Awake()
        {
            if (!StatesSetup) Setup(this, this, -1);
            //if (!Gameplay.Active) return;
            OnAwake();
            DoAwake();

            for (int i = 0; i < behaviors.Length; i++) behaviors[i].OnEnter(null, false);
            TransitionState(this);

            waitforMachineInit?.Invoke();
        }

        private void Reset()
        {
            Transform tryRoot = transform.Find("States");
            if (tryRoot != null) this.NewGameObject("States", parent: transform);
        }

        public void Setup(StateMachine machine, IStateGroup parent, int layer, bool makeDirty = false)
        {
            if (StateHolder == null)
            {
                Transform tryRoot = transform.Find("States");
                StateHolder = tryRoot != null ? tryRoot : throw new System.Exception("State Root Missing");
            }
            if (StateHolder.childCount == 0)
                throw new System.Exception("Stateless State Machines are not supported. If you need to use StateBehaviors on something with only one state, create a dummy state.");

            this.OnSetup();

            {
                childCount = StateHolder.childCount;
                childNodes = new IStateChild[childCount];
                for (int i = 0; i < childCount; i++)
                {
                    childNodes[i] = StateHolder.GetChild(i).GetComponent<IStateChild>();
                    childNodes[i].Setup(machine, this, layer + 1);
                }
            }//Children Setup

            behaviors = GetComponents<StateBehavior>();
            for (int i = 0; i < behaviors.Length; i++) behaviors[i].Setup(this);

            StatesSetup = true;

#if UNITY_EDITOR
            if (makeDirty) EditorUtility.SetDirty(this);
#endif
        }

#if UNITY_EDITOR
        [Button]
        public void ManualSetup()
        {
            if (StatesSetup)
            {
                bool answer = EditorUtility.DisplayDialog("Setup", "This State Machine has already been setup, do you still want to setup again?", "Yes", "No");
                if (!answer) return;
            }

            Setup(this, this, -1);

            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(gameObject.scene);

            EditorUtility.DisplayDialog("Setup Complete", "This State Machine has been setup, be sure to save changes to the prefab.", "Nice.");
        }
#endif


        protected virtual void OnSetup() { }
        protected virtual void OnAwake() { }

        #endregion


        public virtual void TransitionState(IStateGroup groupState)
        {
            IStateNode cursor = groupState.ChildNodes[0];
            while (cursor is IStateGroup group) 
                cursor = group.ChildNodes[0];
            TransitionState(cursor as State);
        }
        public virtual void TransitionState(State nextState)
        {
            if (!Application.isPlaying ||
                nextState == null ||
                nextState == CurrentState
               ) return;

            StateGroup cursorStart = null;
            int startDepth = 0;
            if (CurrentState != null)
            {
                CurrentState.DoExit(nextState);
                cursorStart = CurrentState.Parent as StateGroup;
                startDepth = CurrentState.Layer - 1;
            }

            var cursorTarget = nextState.Parent as StateGroup;
            int targetDepth = nextState.Layer - 1;

            while (startDepth != targetDepth || cursorStart.Parent != cursorTarget.Parent)
            {
                if(startDepth >= targetDepth && cursorStart != null)
                {
                    cursorStart.DoExit(nextState);
                    cursorStart = cursorStart.Parent as StateGroup;
                    startDepth--;
                }
                if(targetDepth <= startDepth)
                {
                    ExitStates.Push(cursorTarget);
                    cursorTarget = cursorTarget.Parent as StateGroup;
                    targetDepth--;
                }
            }

            cursorStart.DoExit(nextState);
            ExitStates.Push(cursorTarget);

            while(ExitStates.Count > 0) 
                ExitStates.Pop().DoEnter(CurrentState, ExitStates.Peek());

            nextState.DoEnter(CurrentState);
            CurrentState = nextState;

        }
        private static Stack<StateGroup> ExitStates = new();









        public void Build() => Setup(this, this, -1, true);
        public void DoAwake()
        {
            for (int i = 0; i < behaviors.Length; i++) behaviors[i].OnAwake();
            for (int i = 0; i < childNodes.Length; i++) childNodes[i].DoAwake();
        }
        public void DoUpdate()
        {
            for (int i = 0; i < behaviors.Length; i++) behaviors[i].OnUpdate();
            if (childCount > 0 && currentChild != null) currentChild.DoUpdate();
        }
        public void DoFixedUpdate()
        {
            for (int i = 0; i < behaviors.Length; i++) behaviors[i].OnFixedUpdate();
            if (childCount > 0 && currentChild != null) currentChild.DoFixedUpdate();
        }
    }
}