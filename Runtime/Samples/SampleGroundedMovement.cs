using SLS.StateMachineH;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SLS.StateMachineH.Samples
{
    public class SampleGroundedMovement : StateBehavior
    {
        public float moveSpeed;
        public SampleAirMovement jumpingState;

        [SerializeField, HideInInspector] private MachineRigidBody body;
        internal override void OnSetup()
        {
            if (body == null) body = GetComponentFromMachine<MachineRigidBody>();
        }

        internal override void OnUpdate()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Debug.Log($"Jumping from position: {transform.position}");
                jumpingState.Jump();
            }
        }
        internal override void OnFixedUpdate()
        {
            if (Input.GetKey(KeyCode.W))
                body.velocity.z = moveSpeed;
            else if (Input.GetKey(KeyCode.S))
                body.velocity.z = -moveSpeed;
            else
                body.velocity.z = 0f;

            if (Input.GetKey(KeyCode.A))
                body.velocity.x = -moveSpeed;
            else if (Input.GetKey(KeyCode.D))
                body.velocity.x = moveSpeed;
            else
                body.velocity.x = 0f;
        }
    }

}
