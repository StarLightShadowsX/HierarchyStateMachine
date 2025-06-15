using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SLS.StateMachineH
{
    public class StateAnimator : StateBehavior
    {
        public enum EntryAnimAction { None, Play, CrossFade, Trigger }
        public EntryAnimAction onEntry;
        public string onEnterName;
        public float onEnterTime;
        public bool doWhenNotFinal;

        [HideInInspector] public Animator animator;

        protected override void OnSetup()
        {
            TryGetComponentFromMachine(out animator);
            if(animator == null) Destroy(this);
        }

        public override void OnEnter(State prev, bool isFinal)
        {
            if (!isFinal && !doWhenNotFinal) return;
            if (onEntry == EntryAnimAction.Play) Play(onEnterName);
            if (onEntry == EntryAnimAction.CrossFade) CrossFade(onEnterName, onEnterTime);
            if (onEntry == EntryAnimAction.Trigger) Trigger(onEnterName);
        }

        public void Play(string name) => animator.Play(name);
        public void CrossFade(string name, float time = 0f) => animator.CrossFade(name, time, 0);
        public void Trigger(string name) => animator.SetTrigger(name);

        public void PlayAtCurrentPoint(string name) => animator.Play(name, -1, animator.GetCurrentAnimatorStateInfo(-1).normalizedTime);
        public void CrossFadeAtCurrentPoint(string name, float time = 0f) => animator.CrossFade(name, time, 0, animator.GetCurrentAnimatorStateInfo(-1).normalizedTime);


    }

}

