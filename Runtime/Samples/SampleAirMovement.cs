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
            body.velocity.y = jumpPower;
            body.Position += Vector3.up * body.groundCheckBuffer;
            body.UnLand(JumpState.Jumping);
            State.Enter();
            body.LandEvent += Land;
        }

        public void Land()
        {
            groundedState.State.Enter();
            body.LandEvent -= Land;
        }



    }

}