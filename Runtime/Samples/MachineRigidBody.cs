using SLS.StateMachineH;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace SLS.StateMachineH.Samples
{
    [RequireComponent(typeof(Rigidbody), typeof(CapsuleCollider), typeof(StateMachine))]
    public class MachineRigidBody : StateBehavior
    {
        [SerializeField] Vector3 defaultGravity = new(0,1,0);
        [SerializeField] float maxSlopeNormalAngle = 45f;
        /// <summary>
        /// Whether this body should automatically check the grounded status before movement.
        /// </summary>
        public bool checkGround = true;
        /// <summary>
        /// The buffer used to check for ground.
        /// </summary>
        public float groundCheckBuffer = 0.1f;
        /// <summary>
        /// The number of steps used in the Collide & Slide Algorithm.
        /// </summary>
        public int movementProjectionSteps = 5;

        [field: SerializeField, HideInInspector] public Rigidbody RB { get; private set; }
        [field: SerializeField, HideInInspector] public CapsuleCollider Collider { get; private set; }

        public Vector3 Position
        {
            get => RB.isKinematic ? transform.position : RB.position;
            set
            {
                if (RB.isKinematic)
                    return;
                transform.position = value;
                RB.position = value;
                RB.MovePosition(value);
            }
        }
        public Quaternion RotationQ
        { get => RB.rotation; set => RB.rotation = value; }
        public Vector3 Rotation
        {
            get => transform.eulerAngles;
            set => transform.eulerAngles = value;
        }

        /// <summary>
        /// Custom velocity value.
        /// </summary>
        [NonSerialized] public Vector3 velocity = new(0, 0, 0);
        /// <summary>
        /// Custom angular velocity value.
        /// </summary>
        [NonSerialized] public Vector3 angularVelocity = new(0, 0, 0);

        /// <summary>
        /// The active direction of the character. Simpler controllers can probably avoid using this.
        /// </summary>
        [NonSerialized] public Vector3 direction = new(0, 0, 1);
        /// <summary>
        /// The active gravity value. (Inverted. y=1 is down.)
        /// </summary>
        [NonSerialized] private Vector3 gravity = new(0, 9.8f, 0);

        ///// <summary>
        /// The influence of the rigidbody on the character's movement. (0 = no influence, 1 = full influence.)
        /// </summary>
        //public float rigidBodyInfluence = 0;
        ///// <summary>
        /// The influence of the rigidbody on the character's rotation. (0 = no influence, 1 = full influence.)
        /// </summary>
        //public float rigidBodyAngularInfluence = 0;


        public JumpState JumpState { get; protected set; } = JumpState.Grounded;
        public bool Grounded => JumpState == JumpState.Grounded;

        protected override void OnSetup()
        {
            if (RB == null) RB = GetComponent<Rigidbody>();
            if (Collider == null) Collider = GetComponent<CapsuleCollider>();
        }

        protected override void OnFixedUpdate()
        {
            /*if (rigidBodyInfluence == 0)*/ RB.velocity = Vector3.zero;
            /*if (rigidBodyAngularInfluence == 0)*/ RB.angularVelocity = Vector3.zero;

            initVelocity = velocity * Time.fixedDeltaTime;
            initNormal = anchorPoint.normal;

            if (checkGround)
            {
                if (GroundCheck(out anchorPoint))
                {
                    initNormal = anchorPoint.normal;
                    if (WithinSlopeAngle(anchorPoint.normal))
                    {
                        Land(anchorPoint);
                        velocity.y = 0;
                        initVelocity.y = 0;
                        initVelocity = initVelocity.ProjectAndScale(anchorPoint.normal);
                    }
                }
                else if (JumpState == JumpState.Grounded) UnLand();
            }

            Move(initVelocity, initNormal);

            if(!Grounded) ApplyGravity();
        }

        Vector3 initVelocity;
        Vector3 initNormal;



        AnchorPoint anchorPoint = new()
        {
            point = Vector3.zero,
            normal = Vector3.up,
            transform = null
        };

        /// <summary>
        /// The Collide and Slide Algorithm.
        /// </summary>
        /// <param name="vel">Input Velocity.</param>
        /// <param name="prevNormal">The Normal of the previous Step.</param>
        /// <param name="step">The current step. Starts at 0.</param>
        private void Move(Vector3 vel, Vector3 prevNormal, int step = 0)
        {
            if (RB.DirectionCast(vel.normalized, vel.magnitude, groundCheckBuffer, out RaycastHit hit))
            {
                Vector3 snapToSurface = vel.normalized * hit.distance;
                Vector3 leftover = vel - snapToSurface;
                Vector3 nextNormal = hit.normal;

                if (step == movementProjectionSteps) return;

                if (!MoveForward(snapToSurface)) return;

                if (Grounded)
                {
                    //Runs into wall/to high incline.
                    if (Mathf.Approximately(hit.normal.y, 0) || (hit.normal.y > 0 && !WithinSlopeAngle(hit.normal)))
                        Stop(hit.normal);

                    if (Grounded && prevNormal.y > 0 && hit.normal.y < 0) //Floor to Cieling
                        FloorCeilingLock(prevNormal, hit.normal);
                    else if (Grounded && prevNormal.y < 0 && hit.normal.y > 0) //Ceiling to Floor
                        FloorCeilingLock(hit.normal, prevNormal);
                }
                else
                {
                    if (vel.y < .1f && WithinSlopeAngle(hit.normal))
                    {
                        Land(hit);
                        leftover.y = 0;
                    }
                    else if (vel.y < -1f && RB.DirectionCastAll(vel, vel.y.Abs(), groundCheckBuffer, out RaycastHit[] downHits) && downHits.Length > 1)
                    {
                        UnLand();
                        leftover.y = 0;
                    }
                    else leftover = leftover.ProjectAndScale(hit.normal);
                }

                void FloorCeilingLock(Vector3 floorNormal, Vector3 ceilingNormal) =>
                    Stop(floorNormal.y != floorNormal.magnitude ? floorNormal : ceilingNormal);

                void Stop(Vector3 newNormal) => nextNormal = newNormal.XZ().normalized;


                Vector3 newDir = leftover.ProjectAndScale(nextNormal) * (Vector3.Dot(leftover.normalized, nextNormal) + 1);
                Move(newDir, nextNormal, step + 1);
            }
            else
            {

                if (step == movementProjectionSteps) return;
                if (!MoveForward(vel)) return;

                //Snap to ground when walking on a downward slope.
                if (Grounded && initVelocity.y <= 0)
                {
                    if (RB.DirectionCast(Vector3.down, 0.5f, groundCheckBuffer, out RaycastHit groundHit))
                        Position += Vector3.down * groundHit.distance;
                    else 
                        UnLand();
                }
            }

            bool MoveForward(Vector3 offset)
            {
                Position += offset;
                return true;
            }
        }

        public void ApplyGravity() => velocity -= gravity * Time.fixedDeltaTime;

        public bool GroundCheck(out AnchorPoint groundHit)
        {
            bool result = RB.DirectionCast(Vector3.down, groundCheckBuffer, groundCheckBuffer, out RaycastHit raycast);
            groundHit = new AnchorPoint(raycast);
            return result;
        }
        public void Land(RaycastHit groundHit)
        {
            if (JumpState == JumpState.Grounded) return;
            JumpState = JumpState.Grounded;
            this.anchorPoint = new AnchorPoint(groundHit);
            LandEvent?.Invoke();
        }
        public void Land(ContactPoint groundHit)
        {
            if (JumpState == JumpState.Grounded) return;
            JumpState = JumpState.Grounded;
            this.anchorPoint = new AnchorPoint(groundHit);
            LandEvent?.Invoke();
        }
        public void Land(AnchorPoint groundHit)
        {
            if (JumpState == JumpState.Grounded) return;
            JumpState = JumpState.Grounded;
            this.anchorPoint = groundHit;
            LandEvent?.Invoke();
        }
        public void UnLand(JumpState newState = JumpState.PastApex)
        {
            JumpState = newState;
            anchorPoint = default;
            anchorPoint.normal = gravity.normalized;
        }
        public Action LandEvent;


        private bool WithinSlopeAngle(Vector3 inNormal) => Vector3.Angle(Vector3.up, inNormal) < maxSlopeNormalAngle;

        private void OnCollisionEnter(Collision collision)
        {
            Vector3 contactPoint = collision.GetContact(0).normal;
            if (!Grounded && velocity.y > .1f && Vector3.Dot(contactPoint, Vector3.up) < -0.75f) velocity.y = 0;
            else if (!Grounded && WithinSlopeAngle(contactPoint)) 
                Land(collision.GetContact(0));

        }





        /// <summary>
        /// Returns the current velocity of the Rigidbody. (Inverted. y=1 is downwards, y=-1 is upwards.)
        /// </summary>
        public Vector3 Get3DGravity() => gravity;
        /// <summary>
        /// Returns the current velocity of the Rigidbody. (Y only.) (Inverted. 1 is downwards, -1 is upwards.)
        /// </summary>
        public float GetGravity() => gravity.y;
        /// <summary>
        /// Sets the current velocity of the Rigidbody. (Inverted. y=1 is downwards, y=-1 is upwards.)
        /// </summary>
        /// <param name="newGravity">The new gravity value.</param>
        public void SetGravity(Vector3 newGravity) => gravity = newGravity;
        /// <summary>
        /// Sets the current velocity of the Rigidbody. (Y only.) (Inverted. 1 is downwards, -1 is upwards.)
        /// </summary>
        /// <param name="newGravity">The new gravity value.</param>
        public void SetGravity(float newGravity) => gravity = new(0, newGravity, 0);
        /// <summary>
        /// Sets the current velocity of the Rigidbody. (Inverted. y=1 is downwards, y=-1 is upwards.)
        /// </summary>
        /// <param name="newX"> The new gravity value on the x axis. (1 = left.) </param>
        /// <param name="newY"> The new gravity value on the y axis. (1 = down.) </param>
        /// <param name="newZ"> The new gravity value on the z axis. (1 = back.) </param>
        public void SetGravity(float newX, float newY, float newZ) => gravity = new(newX, newY, newZ);

    }

    public enum JumpState
    {
        Grounded,
        Jumping,
        PreApex,
        PastApex,
        Falling,
        TerminalVelocity
    }

    public struct AnchorPoint
    {
        public Vector3 point;
        public Vector3 normal;
        public Transform transform;

        public AnchorPoint(RaycastHit hit)
        {
            point = hit.point;
            normal = hit.normal;
            transform = hit.transform;
        }
        public AnchorPoint(ContactPoint contact)
        {
            point = contact.point;
            normal = contact.normal;
            transform = contact.otherCollider.transform;
        }
        public AnchorPoint(Vector3 point, Vector3 normal, Transform transform)
        {
            this.point = point;
            this.normal = normal;
            this.transform = transform;
        }
    }
}