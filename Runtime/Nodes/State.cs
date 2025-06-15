using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

namespace SLS.StateMachineH { 
    public class State : MonoBehaviour
    {
        [field: SerializeField] public StateBehavior[] Behaviors { get; internal set; }
        [field: SerializeField] public StateMachine Machine { get; internal set; }
        [field: SerializeField] public State Parent { get; internal set; }
        [field: SerializeField] public int Layer { get; internal set; }
        [field: SerializeField, SerializeReference] public List<State> Children { get; internal set; } = new();
        [field: SerializeField] public int ChildCount { get; internal set; }

        public virtual bool Active { get; internal set; }
        public State CurrentChild { get; internal set; }

        public virtual StateType Type => ChildCount > 0
            ? StateType.Group
            : StateType.End;
        public virtual bool HasChildren => ChildCount > 0;
        public new string name => gameObject.name;


        public virtual void Setup(StateMachine machine, State parent, int layer, bool makeDirty = false)
        {
            this.Machine = machine;
            this.Parent = parent;
            this.Layer = layer;
            Active = false;
            gameObject.SetActive(false);
            Behaviors = GetComponents<StateBehavior>();
            for (int i = 0; i < Behaviors.Length; i++)
                Behaviors[i].Setup(this, makeDirty);

            {
                ChildCount = transform.childCount;
                Children = new();
                for (int i = 0; i < ChildCount; i++)
                {
                    Children.Add(transform.GetChild(i).GetComponent<State>());
                    Children[i].Setup(machine, this, layer + 1);
                }
            }//Children Setup

#if UNITY_EDITOR
            if (makeDirty) EditorUtility.SetDirty(this);
#endif
        }

        internal void DoAwake()
        {
            for (int i = 0; i < Behaviors.Length; i++)
                Behaviors[i].OnAwake();

            for (int i = 0; i < Children.Count; i++)
                Children[i].DoAwake();
        }

        internal void DoUpdate()
        {
            for (int i = 0; i < Behaviors.Length; i++)
                Behaviors[i].OnUpdate();

            CurrentChild?.DoUpdate();
        }

        internal void DoFixedUpdate()
        {
            for (int i = 0; i < Behaviors.Length; i++)
                Behaviors[i].OnFixedUpdate();

            CurrentChild?.DoFixedUpdate();
        }
        internal void DoEnter(State prev)
        {
            for (int i = 0; i < Behaviors.Length; i++)
                Behaviors[i].OnEnter(null, false);
            Active = true;
            gameObject.SetActive(true);
            for (int i = 0; i < Behaviors.Length; i++)
                Behaviors[i].OnEnter(prev, false);
        }
        internal void DoExit(State next)
        {
            for (int i = 0; i < Behaviors.Length; i++)
                Behaviors[i].OnExit(null);
            Active = false;
            gameObject.SetActive(false);
            CurrentChild = null;
            for (int i = 0; i < Behaviors.Length; i++)
                Behaviors[i].OnExit(next);
        }

        [ContextMenu("Enter")]
        public void Enter() => Machine.TransitionState(this);

        public State AddChildNode()
        {
            GameObject newObject = new("New State");
            newObject.transform.SetParent(this is StateMachine SM ? SM.StateHolder : transform, false);
            State newNode = newObject.AddComponent<State>();
            Children.Add(newNode);
            ChildCount++;
            newNode.Setup(Machine, this, Layer + 1, true);

#if UNITY_EDITOR
            Undo.RegisterCreatedObjectUndo(newObject, "Add Child Node");
            Undo.RecordObject(this, "Add Child Node");
            EditorUtility.SetDirty(Machine);
            EditorUtility.SetDirty(this);
            EditorUtility.SetDirty(newNode);
#endif
            return newNode;
        }
    }

    public enum StateType
    {
        End,
        Group,
        Machine
    }
}