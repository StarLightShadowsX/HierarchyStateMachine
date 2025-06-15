using SLS.StateMachineH;
using UnityEngine;


#if ULT_EVENTS
using EVENT = UltEvents.UltEvent;
#else
using EVENT = UnityEngine.Events.UnityEvent;
#endif

namespace SLS.StateMachineH
{
    public class StateLifeCycleEvents : StateBehavior
    {
        public EVENT onSetup = new();
        public EVENT onAwake = new();
        public EVENT onEnter = new();
        public EVENT onExit = new();
        public GameObject[] activateObjects;

        protected override void OnSetup() => onSetup?.Invoke();
        public override void OnAwake() => onAwake?.Invoke();
        public override void OnEnter(State prev, bool isFinal)
        {
            onEnter?.Invoke();
            if (activateObjects != null)
                for (int i = 0; i < activateObjects.Length; i++)
                    activateObjects[i].SetActive(true);
        }
        public override void OnExit(State next)
        {
            onExit?.Invoke();
            if (activateObjects != null)
                for (int i = 0; i < activateObjects.Length; i++)
                    activateObjects[i].SetActive(false);
        }
    }
}