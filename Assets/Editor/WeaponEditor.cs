using LEGOModelImporter;
using Unity.LEGO.Behaviours.Actions;
using UnityEditor;
using UnityEngine;
using Unity.LEGO.Game;
using System.IO;

namespace Unity.LEGO.EditorExt
{
    [CustomEditor(typeof(Weapons), true)]
    public class WeaponEditor : RepeatableActionEditor
    {
        SpawnAction m_SpawnAction;

        SerializedProperty m_ModelProp;
        SerializedProperty m_SpawnMethodProp;
        SerializedProperty m_EffectProp;
        SerializedProperty m_SpawnAreaShapeProp;
        SerializedProperty m_SpawnAreaCenterProp;
        SerializedProperty m_SpawnAreaSizeProp;
        SerializedProperty m_SpawnAreaRadiusProp;
        SerializedProperty m_SnapToGroundProp;
        SerializedProperty m_SpawnOrientationTypeProp;
        SerializedProperty m_SpawnOrientationProp;
        SerializedProperty m_CollideProp;
        SerializedProperty m_BuildTimeProp;
        SerializedProperty m_WeaponsDataProp;
        SerializedProperty m_ObjectiveWeaponsProp;
        SerializedProperty m_VariableProp;

        Editor m_VariableEditor;

        static readonly Color s_BacksideColour = new Color(0.1f, 1.0f, 0.0f, 0.1f);

        protected override void OnEnable()
        {
            base.OnEnable();

            m_SpawnAction = (SpawnAction)m_Action;

            m_ModelProp = serializedObject.FindProperty("m_Model");
            m_SpawnMethodProp = serializedObject.FindProperty("m_SpawnMethod");
            m_EffectProp = serializedObject.FindProperty("m_Effect");
            m_SpawnAreaShapeProp = serializedObject.FindProperty("m_SpawnAreaShape");
            m_SpawnAreaCenterProp = serializedObject.FindProperty("m_SpawnAreaCenter");
            m_SpawnAreaSizeProp = serializedObject.FindProperty("m_SpawnAreaSize");
            m_SpawnAreaRadiusProp = serializedObject.FindProperty("m_SpawnAreaRadius");
            m_SnapToGroundProp = serializedObject.FindProperty("m_SnapToGround");
            m_SpawnOrientationTypeProp = serializedObject.FindProperty("m_SpawnOrientationType");
            m_SpawnOrientationProp = serializedObject.FindProperty("m_SpawnOrientation");
            m_CollideProp = serializedObject.FindProperty("m_Collide");
            m_BuildTimeProp = serializedObject.FindProperty("m_BuildTime");
            m_WeaponsDataProp = serializedObject.FindProperty("m_WeaponData");
            m_ObjectiveWeaponsProp = serializedObject.FindProperty("m_ObjectiveWeapons");

            m_VariableProp = serializedObject.FindProperty("m_Variable");
        }

        protected override void CreateGUI()
        {

            // Refresh variable list.
            var variables = GetAvailableVariables();
            variables.Item2.Add("[Add New Variable]");

            // Update variable index.
            var index = variables.Item1.FindIndex(item => item == (Variable)m_VariableProp.objectReferenceValue);

            index = EditorGUILayout.Popup(new GUIContent("Variable", "The variable to modify."), index, variables.Item2.ToArray());

            if (index > -1)
            {
                EditorGUI.EndDisabledGroup();

                EditorGUILayout.PropertyField(m_PauseProp);

                EditorGUI.BeginDisabledGroup(EditorApplication.isPlaying);

                EditorGUILayout.PropertyField(m_RepeatProp);

                DrawSeparator();
                EditorGUILayout.LabelField("Variable Settings", EditorStyles.boldLabel);

                if (index == variables.Item2.Count - 1)
                {
                    var newVariable = CreateInstance<Variable>();
                    newVariable.Name = "Variable";
                    var newVariableAssetPath = AssetDatabase.GenerateUniqueAssetPath(Path.Combine(VariableManager.k_VariablePath, "Variable.asset"));
                    AssetDatabase.CreateAsset(newVariable, newVariableAssetPath);
                    m_VariableProp.objectReferenceValue = newVariable;
                }
                else
                {
                    m_VariableProp.objectReferenceValue = variables.Item1[index];

                    // Only recreate editor if necessary.
                    if (!m_VariableEditor || m_VariableEditor.target != m_VariableProp.objectReferenceValue)
                    {
                        DestroyImmediate(m_VariableEditor);
                        m_VariableEditor = CreateEditor(m_VariableProp.objectReferenceValue);
                    }

                    m_VariableEditor.OnInspectorGUI();

                    if (GUILayout.Button("Delete Variable"))
                    {
                        AssetDatabase.DeleteAsset(variables.Item3[index]);
                    }
                }
            }

            EditorGUILayout.PropertyField(m_AudioProp);
            EditorGUILayout.PropertyField(m_AudioVolumeProp);

            EditorGUI.BeginDisabledGroup(EditorApplication.isPlaying);

            if (m_WeaponsDataProp != null)
            {
                EditorGUILayout.PropertyField(m_WeaponsDataProp);
                if (m_ObjectiveWeaponsProp != null)
                    EditorGUILayout.PropertyField(m_ObjectiveWeaponsProp);
            }
            else
            {

                if (m_ModelProp.objectReferenceValue == null)
                {
                    EditorGUILayout.HelpBox("No model selected to spawn.", MessageType.Warning);
                }
                else
                {
                    var modelGO = (GameObject)m_ModelProp.objectReferenceValue;
                    if (!modelGO.GetComponent<Model>() && !modelGO.GetComponent<ModelGroup>() && !modelGO.GetComponent<Brick>())
                    {
                        EditorGUILayout.HelpBox("Only LEGO models can be selected to spawn.", MessageType.Warning);
                    }
                }

                // Only allow non-scene objects to be assigned.
                m_ModelProp.objectReferenceValue = EditorGUILayout.ObjectField(new GUIContent("LEGO Model", "The LEGO model to spawn. Only prefabs are allowed."), m_ModelProp.objectReferenceValue, typeof(GameObject), false);
            }

            EditorGUILayout.PropertyField(m_SpawnMethodProp);

            if ((SpawnAction.SpawnMethod)m_SpawnMethodProp.enumValueIndex != SpawnAction.SpawnMethod.Appear)
            {
                EditorGUILayout.PropertyField(m_EffectProp);
            }

            EditorGUILayout.PropertyField(m_SpawnAreaShapeProp);
            EditorGUILayout.PropertyField(m_SpawnAreaCenterProp, new GUIContent("Center"));

            switch ((SpawnAction.SpawnShape)m_SpawnAreaShapeProp.enumValueIndex)
            {
                case SpawnAction.SpawnShape.Box:
                    EditorGUILayout.PropertyField(m_SpawnAreaSizeProp, new GUIContent("Size"));
                    break;
                case SpawnAction.SpawnShape.Sphere:
                    EditorGUILayout.PropertyField(m_SpawnAreaRadiusProp, new GUIContent("Radius"));
                    break;
            }

            EditorGUILayout.PropertyField(m_SnapToGroundProp);

            EditorGUILayout.PropertyField(m_SpawnOrientationTypeProp, new GUIContent("Orientation"));

            if ((SpawnAction.SpawnOrientation)m_SpawnOrientationTypeProp.enumValueIndex != SpawnAction.SpawnOrientation.Random)
            {
                EditorGUILayout.PropertyField(m_SpawnOrientationProp, new GUIContent("Specific Orientation"));
            }

            EditorGUILayout.PropertyField(m_CollideProp);

            EditorGUI.EndDisabledGroup();

            if ((SpawnAction.SpawnMethod)m_SpawnMethodProp.enumValueIndex == SpawnAction.SpawnMethod.BuildFromAroundModel || (SpawnAction.SpawnMethod)m_SpawnMethodProp.enumValueIndex == SpawnAction.SpawnMethod.BuildFromSpawnAction)
            {
                EditorGUILayout.PropertyField(m_BuildTimeProp);
            }

            EditorGUILayout.PropertyField(m_PauseProp);

            EditorGUI.BeginDisabledGroup(EditorApplication.isPlaying);

            EditorGUILayout.PropertyField(m_RepeatProp);

            EditorGUI.EndDisabledGroup();
        }

        public override void OnSceneGUI()
        {
            base.OnSceneGUI();

            if (Event.current.type == EventType.Repaint)
            {
                if (m_SpawnAction && m_SpawnAction.IsPlacedOnBrick())
                {
                    var spawnPositionWorld = m_SpawnAction.GetBrickCenter() + m_SpawnAction.GetBrickRotation() * m_SpawnAreaCenterProp.vector3Value;
                    var worldToLocalRotation = Quaternion.Inverse(m_SpawnAction.GetBrickRotation());

                    // Draw orientation arrow.
                    Handles.color = Color.green;
                    if ((SpawnAction.SpawnOrientation)m_SpawnOrientationTypeProp.enumValueIndex != SpawnAction.SpawnOrientation.Random)
                    {
                        var start = spawnPositionWorld;
                        var direction = Quaternion.Euler(m_SpawnOrientationProp.vector3Value) * Vector3.forward;
                        var end = start + direction * 3.2f;
                        Handles.DrawDottedLine(start, end, 5.0f);
                    }

                    // Draw spawn area.
                    Handles.matrix = Matrix4x4.TRS(spawnPositionWorld, m_SpawnAction.GetBrickRotation(), Vector3.one);
                    switch ((SpawnAction.SpawnShape)m_SpawnAreaShapeProp.enumValueIndex)
                    {
                        case SpawnAction.SpawnShape.Box:
                            {
                                Handles.DrawWireCube(Vector3.zero, m_SpawnAreaSizeProp.vector3Value);
                                break;
                            }
                        case SpawnAction.SpawnShape.Sphere:
                            {
                                var radius = m_SpawnAreaRadiusProp.floatValue;

                                // Plane-plane intersections.
                                var cameraPlaneNormal = worldToLocalRotation * Camera.current.transform.forward;
                                var xyPlaneNormal = worldToLocalRotation * Vector3.forward;
                                var xyDirection = Vector3.Cross(xyPlaneNormal, cameraPlaneNormal).normalized * radius;
                                var xzPlaneNormal = worldToLocalRotation * Vector3.up;
                                var xzDirection = Vector3.Cross(xzPlaneNormal, cameraPlaneNormal).normalized * radius;
                                var yzPlaneNormal = worldToLocalRotation * Vector3.right;
                                var yzDirection = Vector3.Cross(yzPlaneNormal, cameraPlaneNormal).normalized * radius;

                                // Draw outline.
                                Handles.DrawWireDisc(Vector3.zero, cameraPlaneNormal, radius);

                                // Draw frontside.
                                Handles.DrawWireArc(Vector3.zero, xyPlaneNormal, xyDirection, 180.0f, radius);
                                Handles.DrawWireArc(Vector3.zero, xzPlaneNormal, xzDirection, 180.0f, radius);
                                Handles.DrawWireArc(Vector3.zero, yzPlaneNormal, yzDirection, 180.0f, radius);

                                // Draw backside.
                                Handles.color = s_BacksideColour;
                                Handles.DrawWireArc(Vector3.zero, xyPlaneNormal, -xyDirection, 180.0f, radius);
                                Handles.DrawWireArc(Vector3.zero, xzPlaneNormal, -xzDirection, 180.0f, radius);
                                Handles.DrawWireArc(Vector3.zero, yzPlaneNormal, -yzDirection, 180.0f, radius);
                                break;
                            }
                        case SpawnAction.SpawnShape.Point:
                            {
                                var cameraPlaneNormal = worldToLocalRotation * Camera.current.transform.forward;
                                Handles.DrawSolidDisc(Vector3.zero, cameraPlaneNormal, 0.16f);
                                break;
                            }
                    }
                }
            }
        }
    }
}
