using UnityEngine;

namespace SLS.StateMachineH
{
    [RequireComponent(typeof(Rigidbody), typeof(StateMachine))]
    public class StatePhysicsCaller2D : StateBehavior
    {
        private IStateBehaviorPhysicsCollision2D[] collisions2D;
        private IStateBehaviorPhysicsTrigger2D[] triggers2D;

        internal override void OnAwake()
        {
            collisions2D = Machine.StateHolder.GetComponentsInChildren<IStateBehaviorPhysicsCollision2D>();
            triggers2D = Machine.StateHolder.GetComponentsInChildren<IStateBehaviorPhysicsTrigger2D>();
        }


        private void OnCollisionEnter2D(Collision2D collision)
        {
            for (int i = 0; i < collisions2D.Length; i++)
                if (collisions2D[i].isActive)
                    collisions2D[i].OnCollisionEnter2D(collision);
        }
        private void OnCollisionExit2D(Collision2D collision)
        {
            for (int i = 0; i < collisions2D.Length; i++)
                if (collisions2D[i].isActive)
                    collisions2D[i].OnCollisionExit2D(collision);
        }
        private void OnTriggerEnter2D(Collider2D collision)
        {
            for (int i = 0; i < triggers2D.Length; i++)
                if (triggers2D[i].isActive)
                    triggers2D[i].OnTriggerEnter2D(collision);

        }
        private void OnTriggerExit2D(Collider2D collision)
        {
            for (int i = 0; i < triggers2D.Length; i++)
                if (triggers2D[i].isActive)
                    triggers2D[i].OnTriggerExit2D(collision);
        }
    }

}