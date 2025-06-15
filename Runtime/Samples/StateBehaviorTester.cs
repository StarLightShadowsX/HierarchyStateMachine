using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SLS.StateMachineH;
public class StateBehaviorTester : StateBehavior
{
    public static Dictionary<string, int> frames;




    internal override void OnAwake() => frames.Add(State.gameObject.name, 0);

    internal override void OnEnter(State prev, bool isFinal)
    {
        Debug.Log($"State {State.gameObject.name} has been entered"); 
    }
    internal override void OnExit(State next)
    {
        Debug.Log($"State {State.gameObject.name} has been exited");
    }

    internal override void OnUpdate() => frames[State.gameObject.name]++;
}
