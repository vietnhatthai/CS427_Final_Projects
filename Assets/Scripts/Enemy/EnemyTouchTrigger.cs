using Unity.LEGO.Behaviours.Actions;
using UnityEngine;

namespace Unity.LEGO.Behaviours.Triggers
{
    public class EnemyTouchTrigger : TouchTrigger
    {
        protected override void Reset()
        {
            base.Reset();

            m_IconPath = "Assets/LEGO/Gizmos/LEGO Behaviour Icons/Touch Trigger.png";
        }

        protected override void Start()
        {
            base.Start();

            if (IsPlacedOnBrick())
            {
                // Add SensoryCollider to all brick colliders.
                foreach (var brick in m_ScopedBricks)
                {
                    foreach (var part in brick.parts)
                    {
                        foreach (var collider in part.colliders)
                        {
                            var sensoryCollider = LEGOBehaviourCollider.Add<SensoryCollider>(collider, m_ConnectedBricks, 0.64f);
                            SetupSensoryCollider(sensoryCollider);
                        }
                    }
                }
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

        protected new void SensoryColliderActivated(SensoryCollider collider, Collider other)
        {
            string name = other.gameObject.name;
            bool isHQ = name == "HQ";

            GameObject modelGO = gameObject;
            BrickColliderCombiner.CombineColliders(modelGO);
            var behaviours = modelGO.GetComponentsInChildren<LEGOBehaviour>();
            foreach (var behaviour in behaviours)
            {
                if (behaviour.GetType() == typeof(EnemyMoveAction))
                {
                    behaviour.GetComponent<EnemyMoveAction>().ExplodeAction(isHQ);
                }
            }
        }

        protected new void SensoryColliderDeactivated(SensoryCollider collider)
        {
            //m_ActiveColliders.Remove(collider);
        }
    }
}