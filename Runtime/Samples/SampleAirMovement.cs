using SLS.StateMachineH;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SLS.StateMachineH.Samples
{
    public class SampleAirMovement : StateBehavior
    {

        public float jumpPower;
        public SampleGroundedMovement groundedState;

        [SerializeField, HideInInspector] private MachineRigidBody body;
        protected override void OnSetup()
        {
            if (body == null) body = GetComponentFromMachine<MachineRigidBody>();
        }

        public void Jump()
        {
            State.Enter(); // Enter the jumping state
            body.velocity.y = jumpPower; // Set the vertical velocity of the player to jumpPower.
            body.UnLand(JumpState.Jumping); //Tell the MachineRigidBody it is no longer grounded and set its "JumpState".
            body.LandEvent += Land; // Temporarily subscribe to the Land event so that Landing logic can be handled here.
        }

        public void Land()
        {
            groundedState.State.Enter();
            body.LandEvent -= Land;
        }



    }

}