using LEGOMaterials;
using LEGOModelImporter;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Unity.LEGO.Behaviours.Triggers
{
    public class TouchColor : SensoryTrigger
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

        protected void SetupSensoryCollider(SensoryCollider collider)
        {
            collider.OnSensorActivated += SensoryColliderActivated;
            collider.OnSensorDeactivated += SensoryColliderDeactivated;

            collider.Sense = m_Sense;
            if (m_Sense == Sense.Tag)
            {
                collider.Tag = m_SenseTag;
            }
        }

        protected void SensoryColliderActivated(SensoryCollider collider, Collider _)
        {
            List<Part> parts = GetComponent<Brick>().parts;
            foreach (Part part in parts)
            {
                part.materialIDs[0] = (int) MouldingColour.Id.BrightRed;
            }
            GetComponent<Brick>().SetMaterial(false);
        }

        protected void SensoryColliderDeactivated(SensoryCollider collider)
        {
            List<Part> parts = GetComponent<Brick>().parts;
            foreach (Part part in parts)
            {
                part.materialIDs[0] = (int)MouldingColour.Id.BrightGreen;
            }
            GetComponent<Brick>().SetMaterial(false);
        }
    }
}
