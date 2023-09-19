using LEGOMaterials;
using LEGOModelImporter;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Unity.LEGO.Behaviours.Triggers
{
    public class TouchColor : SensoryTrigger
    {
        [SerializeField, Tooltip("Color to change to when triggered.")]
        protected MouldingColour.Id m_MouldingColour = MouldingColour.Id.BrightBlue;

        private List<int> color_original = new List<int>();
        private new Brick m_Brick;

        public static event System.Action<Brick> onDestroy;
        private static Material transparentMaterial;

        protected override void Reset()
        {
            base.Reset();

            m_IconPath = "Assets/LEGO/Gizmos/LEGO Behaviour Icons/Touch Trigger.png";
        }

        protected override void Start()
        {
            base.Start();

            m_Brick = GetComponent<Brick>();
            List<Part> parts = m_Brick.parts;

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

        protected new void SensoryColliderActivated(SensoryCollider collider, Collider _)
        {
            List<Part> parts = GetComponent<Brick>().parts;
            foreach (Part part in parts)
            {
                part.materialIDs[0] = (int)m_MouldingColour;
            }

            SetMaterial(true);
        }

        protected new void SensoryColliderDeactivated(SensoryCollider collider)
        {
            List<Part> parts = GetComponent<Brick>().parts;
            for (int i = 0; i < parts.Count; i++)
            {
                parts[i].materialIDs[0] = color_original[i];
            }
            SetMaterial(true);
        }

        private Material GetMaterial(int id)
        {
            // FIXME Remove when colour palette experiments are over.
#if UNITY_EDITOR
            var useBI = MouldingColour.GetBI();
#else
            var useBI = true;
#endif
            var path = MaterialPathUtility.GetPath((MouldingColour.Id)id, false, useBI);
            if (File.Exists(path))
            {
#if UNITY_EDITOR
                return AssetDatabase.LoadAssetAtPath<Material>(path);
#else
                return Resources.Load<Material>(path);  
#endif
            }
            else
            {
                path = MaterialPathUtility.GetPath((MouldingColour.Id)id, true, useBI);
#if UNITY_EDITOR
                return AssetDatabase.LoadAssetAtPath<Material>(path);
#else
                return Resources.Load<Material>(path);
#endif
            }
        }

        public void SetMaterial(bool ghosted, bool recordUndo = true)
        {
            if (ghosted && transparentMaterial == null)
            {
#if UNITY_EDITOR
                transparentMaterial = AssetDatabase.LoadAssetAtPath<Material>("Packages/com.unity.lego.modelimporter/Materials/LEGO_GhostedBrick.mat"); ;
#else
                transparentMaterial = Resources.Load<Material>("Packages/com.unity.lego.modelimporter/Materials/LEGO_GhostedBrick.mat");  
#endif
            }
            foreach (var part in m_Brick.parts)
            {
                if (part.transform.childCount > 0)
                {
                    var colourChangeSurfaces = part.transform.Find("ColourChangeSurfaces");
                    Material material = ghosted ? transparentMaterial : GetMaterial(part.materialIDs[0]);
                    if (!material)
                    {
                        continue;
                    }

                    var renderersToEdit = new List<MeshRenderer>();
                    var colourSurfaceRenderersToEdit = new List<(MeshRenderer, int)>();

                    var shell = part.transform.Find("Shell");
                    if (shell)
                    {
                        var mr = shell.GetComponent<MeshRenderer>();
                        renderersToEdit.Add(mr);
                    }

                    foreach (var knob in part.knobs)
                    {
                        var mr = knob.GetComponent<MeshRenderer>();
                        renderersToEdit.Add(mr);
                    }

                    foreach (var tube in part.tubes)
                    {
                        var mr = tube.GetComponent<MeshRenderer>();
                        renderersToEdit.Add(mr);
                    }

                    if (part.materialIDs.Count > 1 && colourChangeSurfaces)
                    {
                        for (var i = 1; i < part.materialIDs.Count; i++)
                        {
                            var surface = colourChangeSurfaces.GetChild(i - 1);
                            if (surface)
                            {
                                var mr = surface.GetComponent<MeshRenderer>();
                                colourSurfaceRenderersToEdit.Add((mr, part.materialIDs[i]));
                            }
                        }
                    }
                    if (recordUndo)
                    {
#if UNITY_EDITOR
                        Undo.RegisterCompleteObjectUndo(renderersToEdit.ToArray(), "Recording material changes");
                        Undo.RegisterCompleteObjectUndo(colourSurfaceRenderersToEdit.Select(x => x.Item1).ToArray(), "Recording colour surface material changes");
#endif
                    }
                    foreach (var renderer in renderersToEdit)
                    {
                        renderer.sharedMaterial = material;
                    }

                    foreach (var (cs, id) in colourSurfaceRenderersToEdit)
                    {
                        if (ghosted)
                        {
                            cs.sharedMaterial = material;
                        }
                        else
                        {
                            cs.sharedMaterial = GetMaterial(id);
                        }
                    }
                }
            }
        }

        public void UpdateColliding(bool isColliding, bool updateMaterial = true, bool recordUndo = true)
        {
            bool changed = m_Brick.colliding != isColliding;
            if (recordUndo && changed)
            {
#if UNITY_EDITOR
                Undo.RegisterCompleteObjectUndo(this, "Changing collision status.");
#endif
            }
            m_Brick.colliding = isColliding;

            if (updateMaterial && changed)
            {
                SetMaterial(m_Brick.colliding, recordUndo);
            }
            Connection.RegisterPrefabChanges(this);
        }

        public bool IsColliding(out int hits, HashSet<Brick> ignoredBricks = null, bool earlyOut = true)
        {
            foreach (var part in m_Brick.parts)
            {
                if (Part.IsColliding(part, part.transform.localToWorldMatrix, BrickBuildingUtility.ColliderBuffer, out hits, ignoredBricks, earlyOut))
                {
                    return true;
                }
            }
            hits = 0;
            return false;
        }
    }
}
