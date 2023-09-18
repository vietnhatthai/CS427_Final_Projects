using Unity.LEGO.Behaviours.Controls;
using UnityEngine;

namespace Unity.LEGO.Behaviours.Actions
{
    public class EnemyMoveAction : MovementAction
    {
        [SerializeField, Tooltip("The Speed in LEGO modules.")]
        float m_Speed = 15f;

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

        float rotationSpeed, _rotationSpeed;
        int waypointIndex = 0;
        private WaypointManager m_WaypointManager;

        ControlMovement m_ControlMovement;

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
        }


        protected override void Reset()
        {
            base.Reset();

            m_IconPath = "Assets/LEGO/Gizmos/LEGO Behaviour Icons/Move Action.png";
        }

        void FixedUpdate()
        {
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
    }
}