using Unity.LEGO.Behaviours.Actions;
using UnityEngine;

namespace Unity.LEGO.Behaviours.Triggers
{
    public class DefenseNearbyTrigger : SensoryTrigger
    {
        [SerializeField, Tooltip("The distance in LEGO modules.")]
        int m_Distance = 10;

        private Transform m_TargetTransform;

        protected override void Reset()
        {
            base.Reset();

            m_IconPath = "Assets/LEGO/Gizmos/LEGO Behaviour Icons/Nearby Trigger.png";
        }

        protected override void Start()
        {
            base.Start();

            if (IsPlacedOnBrick())
            {
                // Add one SensoryCollider based on scope pivot.
                var colliderComponentToClone = gameObject.AddComponent<SphereCollider>();
                colliderComponentToClone.center = m_ScopedPivotOffset;
                colliderComponentToClone.radius = 0.0f;
                colliderComponentToClone.enabled = false;

                var sensoryCollider = LEGOBehaviourCollider.Add<SensoryCollider>(colliderComponentToClone, m_ConnectedBricks, m_Distance * LEGOBehaviour.LEGOHorizontalModule);
                SetupSensoryCollider(sensoryCollider);

                Destroy(colliderComponentToClone);
            }
        }

        protected new void SetupSensoryCollider(SensoryCollider collider)
        {
            collider.OnSensorActivated += SensoryColliderActivated;
            collider.OnSensorDeactivated += SensoryColliderDeactivated;

            collider.Sense = m_Sense;
            if (m_Sense == Sense.Tag)
            {
                collider.Tag = m_SenseTag;
            }
        }

        protected new void SensoryColliderActivated(SensoryCollider collider, Collider targetCollider)
        {
            GameObject m_Target = targetCollider.gameObject;
            m_Target.GetComponent<EnemyTag>().m_SensoryCollider = collider;
            m_Target.GetComponent<EnemyTag>().OnSensorDeactivated += (SensoryCollider collider) =>
            {
                m_ActiveColliders.Remove(collider);
            };

            m_TargetTransform = targetCollider.transform;
            if (m_TargetTransform)
            {
                GameObject modelGO = gameObject;
                BrickColliderCombiner.CombineColliders(modelGO);
                var behaviours = modelGO.GetComponentsInChildren<LEGOBehaviour>();
                foreach (var behaviour in behaviours)
                {
                    if (behaviour.GetType() == typeof(DefenseLookAtAction))
                    {
                        behaviour.GetComponent<DefenseLookAtAction>().m_TransformModeTransform = m_TargetTransform;
                    }
                }
            }
            m_ActiveColliders.Add(collider);
        }

        protected new void SensoryColliderDeactivated(SensoryCollider collider)
        {
            m_ActiveColliders.Remove(collider);
        }
    }
}
