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
        [SerializeField, Tooltip("Color to change to when triggered.")]
        protected MouldingColour.Id m_MouldingColour = MouldingColour.Id.BrightBlue;

        private List<int> color_original = new List<int>();
        protected override void Reset()
        {
            base.Reset();

            m_IconPath = "Assets/LEGO/Gizmos/LEGO Behaviour Icons/Touch Trigger.png";
        }

        protected override void Start()
        {
            base.Start();

            List<Part> parts = GetComponent<Brick>().parts;
            foreach (Part part in parts)
            {
                color_original.Add(part.materialIDs[0]);
            }

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
                part.materialIDs[0] = (int)m_MouldingColour;
            }
            GetComponent<Brick>().SetMaterial(false);
        }

        protected void SensoryColliderDeactivated(SensoryCollider collider)
        {
            List<Part> parts = GetComponent<Brick>().parts;
            for (int i = 0; i < parts.Count; i++)
            {
                parts[i].materialIDs[0] = color_original[i];
            }
            GetComponent<Brick>().SetMaterial(false);
        }
    }
}
