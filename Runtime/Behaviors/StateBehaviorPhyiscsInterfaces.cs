using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SLS.StateMachineH
{
    public interface IStateBehaviorPhysics
    {
        public sealed bool isActive => (this as StateBehavior).State.Active;
    }
    public interface IStateBehaviorPhysicsCollision : IStateBehaviorPhysics
    {
        void OnCollisionEnter(Collision collision);
        void OnCollisionExit(Collision collision);
    }
    public interface IStateBehaviorPhysicsTrigger : IStateBehaviorPhysics
    {
        void OnTriggerEnter(Collider other);
        void OnTriggerExit(Collider other);
    }
    public interface IStateBehaviorPhysicsCollision2D : IStateBehaviorPhysics
    {
        void OnCollisionEnter2D(Collision2D collision);
        void OnCollisionExit2D(Collision2D collision);
    }
    public interface IStateBehaviorPhysicsTrigger2D : IStateBehaviorPhysics
    {
        void OnTriggerEnter2D(Collider2D collision);
        void OnTriggerExit2D(Collider2D collision);
    }
}