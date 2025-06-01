using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static Unity.VisualScripting.Metadata;

namespace SLS.StateMachineH.V5
{
    public class StateGroup : MonoBehaviour, IStateGroup, IStateChild
    {
        public StateBehavior[] Behaviors => behaviors;
        [SerializeField] protected StateBehavior[] behaviors;
        public IStateChild[] ChildNodes => childNodes;
        [SerializeField] protected IStateChild[] childNodes;
        public int ChildCount => childCount;
        [SerializeField] protected int childCount;
        public IStateChild CurrentChild => currentChild;
        protected IStateChild currentChild;
        public StateMachine Machine => machine;
        [SerializeField] protected StateMachine machine;
        public IStateGroup Parent => parent;
        [SerializeField] protected IStateGroup parent;
        public int Layer => layer;
        [SerializeField] protected int layer;
        public bool Active => active;
        protected bool active;


        public void Setup(StateMachine machine, IStateGroup parent, int layer, bool makeDirty = false)
        {
            this.machine = machine;
            this.parent = parent;
            this.layer = layer;
            active = false;
            gameObject.SetActive(false);
            behaviors = GetComponents<StateBehavior>();
            for (int i = 0; i < behaviors.Length; i++)
                behaviors[i].Setup(this, makeDirty);

            {
                childCount = transform.childCount;
                childNodes = new IStateChild[childCount];
                for (int i = 0; i < childCount; i++)
                {
                    childNodes[i] = transform.GetChild(i).GetComponent<IStateChild>();
                    childNodes[i].Setup(machine, this, layer + 1);
                }
            }//Children Setup

#if UNITY_EDITOR
            if (makeDirty) EditorUtility.SetDirty(this);
#endif
        }



        public void DoAwake()
        {
            for (int i = 0; i < behaviors.Length; i++)
                behaviors[i].OnAwake();

            for (int i = 0; i < childNodes.Length; i++)
                childNodes[i].DoAwake();
        }

        public void DoUpdate()
        {
            for (int i = 0; i < behaviors.Length; i++)
                behaviors[i].OnUpdate();

            currentChild?.DoUpdate();
        }

        public void DoFixedUpdate()
        {
            for (int i = 0; i < behaviors.Length; i++)
                behaviors[i].OnFixedUpdate();

            currentChild?.DoFixedUpdate();
        }

        public void DoEnter(State prev, IStateChild target)
        {
            for (int i = 0; i < behaviors.Length; i++)
                behaviors[i].OnEnter(null, false);
            active = true;
            gameObject.SetActive(true);
            currentChild = target;
            for (int i = 0; i < behaviors.Length; i++)
                behaviors[i].OnEnter(prev, false);
        }

        public void DoExit(State next)
        {
            for (int i = 0; i < behaviors.Length; i++)
                Behaviors[i].OnExit(null);
            active = false;
            gameObject.SetActive(false);
            currentChild = null;
            for (int i = 0; i < behaviors.Length; i++)
                behaviors[i].OnExit(next);
        }
    }
}
