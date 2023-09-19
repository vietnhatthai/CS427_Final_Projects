using System.Collections.Generic;
using Unity.LEGO.Behaviours.Controls;
using Unity.LEGO.Game;
using UnityEngine;

namespace Unity.LEGO.Behaviours.Actions
{
    public class EnemyMoveAction : MovementAction
    {
        [SerializeField, Tooltip("The Speed in LEGO modules.")]
        public float m_Speed = 15f;

        [SerializeField, Range(1, 50), Tooltip("The power of the explosion.")]
        uint m_Power = 10;
        [SerializeField, Tooltip("Remove bricks shortly after the explosion.")]
        bool m_RemoveBricks = false;

        [SerializeField]
        int m_MinSpeed = -20;
        [SerializeField]
        int m_MaxSpeed = 20;
        [SerializeField, Tooltip("The idle speed in LEGO modules per second.")]
        int m_IdleSpeed = 0;
        [SerializeField, Range(0, 720), Tooltip("The rotation speed in degrees per second.")]
        int m_RotationSpeed = 360;
        [SerializeField, Range(0.0f, 50.0f)]
        int m_JumpSpeed = 20;
        [SerializeField]
        uint m_MaxJumpsInAir = 0;
        [SerializeField, Range(0.0f, 80.0f)]
        float m_Gravity = 40;

        [SerializeField, Tooltip("The variable to minus one from when the enemy is destroyed.")]
        public Game.Variable m_MinusVariable = default;

        [SerializeField, Tooltip("The variable to bonus when the enemy is destroyed.")]
        public Game.Variable m_BonusVariable = default;

        int waypointIndex = 0;
        private WaypointManager m_WaypointManager;
        bool m_Detonated;

        public int m_Bonus = 0;
        public int m_Health = 100;

        public GameObject m_PrefabTag;

        ControlMovement m_ControlMovement;
        List<LEGOBehaviour> m_Behaviours = new List<LEGOBehaviour>();

        protected override void Start()
        {
            base.Start();
            m_Repeat = true;

            m_WaypointManager = FindObjectOfType<WaypointManager>();
            m_ControlMovement = gameObject.AddComponent<Character>();

            m_ControlMovement.Setup(
                m_Group,
                m_ScopedBricks,
                m_ScopedPartRenderers,
                m_BrickPivotOffset,
                m_ScopedBounds,
                false,
                true,
                m_Gravity
                );

            if (IsPlacedOnBrick())
            {
                // Find all LEGOBehaviours in scope.
                foreach (var brick in m_ScopedBricks)
                {
                    m_Behaviours.AddRange(brick.GetComponentsInChildren<LEGOBehaviour>());
                }
            }
        }


        protected override void Reset()
        {
            base.Reset();

            m_IconPath = "Assets/LEGO/Gizmos/LEGO Behaviour Icons/Move Action.png";
        }

        void FixedUpdate()
        {
            m_Active = true;
            if (m_Active && m_WaypointManager != null && m_WaypointManager.Count() > 0 && waypointIndex < m_WaypointManager.Count())
            {
                if (IsColliding())
                {
                }
                else
                {
                    // Play audio.
                    if (m_PlayAudio)
                    {
                        PlayAudio();
                        m_PlayAudio = false;
                    }

                    Vector3 targetPosition = m_WaypointManager.GetWaypointPosition(waypointIndex);
                    Vector3 currentPosition = m_Group.transform.position;
                    // move towards target
                    Vector3 m_TargetDirection = targetPosition - currentPosition;
                    m_TargetDirection.Normalize();

                    // Move and rotate control movement.
                    //m_ControlMovement.Movement(m_TargetDirection, m_MinSpeed, m_MaxSpeed, m_IdleSpeed, 0, 0);

                    if (m_PrefabTag)
                    {
                        //m_PrefabTag.transform.position = m_Group.transform.position + Vector3.up + Vector3.forward * 3;
                        m_PrefabTag.transform.localPosition = Vector3.up + Vector3.forward * 3;
                    }

                    m_Group.transform.position += m_TargetDirection * m_Speed * Time.deltaTime;
                    m_ControlMovement.Rotation(m_TargetDirection, m_RotationSpeed);


                    // Update model position.
                    m_MovementTracker.UpdateModelPosition();

                    // check if we have reached the target
                    if (Vector3.Distance(currentPosition, targetPosition) < 3f)
                    {
                        waypointIndex++;
                    }
                }
            }
        }

        protected override bool IsColliding()
        {
            if (base.IsColliding())
            {
                foreach (var activeColliderPair in m_ActiveColliderPairs)
                {
                    if (Physics.ComputePenetration(activeColliderPair.Item1, activeColliderPair.Item1.transform.position, activeColliderPair.Item1.transform.rotation,
                        activeColliderPair.Item2, activeColliderPair.Item2.transform.position, activeColliderPair.Item2.transform.rotation,
                        out Vector3 direction, out _))
                    {
                        if (Vector3.Dot(direction, transform.forward) < -0.0001f)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        protected override void OnDrawGizmos()
        {
            base.OnDrawGizmos();

            if (m_Active && m_WaypointManager != null && m_WaypointManager.Count() > 0 && waypointIndex < m_WaypointManager.Count())
            {
                Vector3 targetPosition = m_WaypointManager.GetWaypointPosition(waypointIndex);
                Vector3 currentPosition = m_Group.transform.position;

                // move towards target
                Vector3 direction = targetPosition - currentPosition;
                direction.Normalize();

                // draw line to target
                Gizmos.color = Color.green;
                Gizmos.DrawLine(currentPosition, targetPosition);

                // draw target
                Gizmos.DrawSphere(targetPosition, 0.2f);

                // draw direction
                Gizmos.color = Color.blue;
                Gizmos.DrawLine(currentPosition, currentPosition + direction);
            }

            // draw forward direction
            Vector3 forward = transform.forward;
            forward.Normalize();
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, transform.position + forward * m_Speed);
            Gizmos.DrawSphere(transform.position + forward * m_Speed, 0.2f);
        }

        public void ExplodeAction(bool isHQ)
        {
            if (m_PrefabTag)
            {
                Destroy(m_PrefabTag);
            }

            if (!m_Detonated)
            {
                // Remove all game objects with LEGOBehaviourCollider components.
                foreach (var brick in m_ScopedBricks)
                {
                    foreach (var behaviourCollider in brick.GetComponentsInChildren<LEGOBehaviourCollider>())
                    {
                        Destroy(behaviourCollider.gameObject);
                    }

                    // Restore part's original colliders.
                    foreach (var part in brick.parts)
                    {
                        BrickColliderCombiner.RestoreOriginalColliders(part);
                    }
                }

                var lift = m_Power * 0.25f;

                // Send all bricks in scope flying.
                foreach (var brick in m_ScopedBricks)
                {
                    brick.DisconnectAll();

                    var rigidBody = brick.gameObject.GetComponent<Rigidbody>();
                    if (!rigidBody)
                    {
                        rigidBody = brick.gameObject.AddComponent<Rigidbody>();
                    }
                    rigidBody.AddExplosionForce(m_Power, transform.position + transform.TransformVector(m_BrickPivotOffset), m_ScopedBounds.extents.magnitude, lift, ForceMode.VelocityChange);

                    if (m_RemoveBricks)
                    {
                        brick.gameObject.AddComponent<BlinkAndDisable>();
                    }
                }

                PlayAudio(moveWithScope: false, destroyWithAction: false);

                // Delay destruction of LEGOBehaviours one frame to allow multiple Explode Actions to detonate.
                m_Detonated = true;

                if (!isHQ)
                {
                    VariableManager.SetValue(m_BonusVariable, VariableManager.GetValue(m_BonusVariable) + m_Bonus);
                }

                if (VariableManager.GetValue(m_MinusVariable) > 0)
                {
                    VariableManager.SetValue(m_MinusVariable, VariableManager.GetValue(m_MinusVariable) - 1);
                }
            }
            else
            {
                // Destroy all the LEGOBehaviours in scope (including this script).
                foreach (var behaviour in m_Behaviours)
                {
                    if (behaviour)
                    {
                        Destroy(behaviour);
                    }
                }
            }
        }
    }
}