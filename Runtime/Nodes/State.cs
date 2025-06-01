using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace SLS.StateMachineH.V5 { 
    public class State : MonoBehaviour, IStateChild
    {

        public StateBehavior[] Behaviors => behaviors;
        [SerializeField] protected StateBehavior[] behaviors;
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

#if UNITY_EDITOR
            if (makeDirty) EditorUtility.SetDirty(this);
#endif
        }

        public void DoAwake()
        {
            for (int i = 0; i < behaviors.Length; i++)
                behaviors[i].OnAwake();
        }
        public void DoUpdate()
        {
            for (int i = 0; i < behaviors.Length; i++)
                behaviors[i].OnUpdate();
        }
        public void DoFixedUpdate()
        {
            for (int i = 0; i < behaviors.Length; i++)
                behaviors[i].OnFixedUpdate();
        }
        public void DoEnter(State prev)
        {
            for (int i = 0; i < behaviors.Length; i++)
                behaviors[i].OnEnter(null, false);
            active = true;
            gameObject.SetActive(true);
            for (int i = 0; i < behaviors.Length; i++) 
                behaviors[i].OnEnter(prev, true);
        }
        public void DoExit(State next)
        {
            for (int i = 0; i < behaviors.Length; i++)
                behaviors[i].OnExit(null);
            active = false;
            gameObject.SetActive(false);
            for (int i = 0; i < behaviors.Length; i++)
                behaviors[i].OnExit(next);
        }
    }
}