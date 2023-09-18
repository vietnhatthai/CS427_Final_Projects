using UnityEditor;
using UnityEngine;
using Unity.LEGO.Behaviours.Actions;
using LEGOModelImporter;
using System.IO;
using Unity.LEGO.Game;

namespace Unity.LEGO.EditorExt
{
    [CustomEditor(typeof(EnemyController), true)]
    public class EnemyControllerEditor : RepeatableActionEditor
    {
        EnemyController m_EnemyController;
        Editor m_VariableEditor;

        SerializedProperty m_VariableProp;
        SerializedProperty m_EnemySettingsProp;
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

        static readonly Color s_BacksideColour = new Color(0.1f, 1.0f, 0.0f, 0.1f);

        protected override void OnEnable()
        {
            base.OnEnable();

            m_EnemyController = (EnemyController)m_Action;

            m_VariableProp = serializedObject.FindProperty("m_Variable");
            m_EnemySettingsProp = serializedObject.FindProperty("m_EnemySettings");
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
        }

        protected override void CreateGUI()
        {
            EditorGUILayout.PropertyField(m_AudioProp);
            EditorGUILayout.PropertyField(m_AudioVolumeProp);

            EditorGUI.BeginDisabledGroup(EditorApplication.isPlaying);

            EditorGUILayout.PropertyField(m_EnemySettingsProp);

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

            EditorGUI.BeginDisabledGroup(EditorApplication.isPlaying);

            if (m_VariableProp == null)
            {
                EditorGUI.EndDisabledGroup();
                return;
            }

            // Refresh variable list.
            var variables = GetAvailableVariables();
            variables.Item2.Add("[Add New Variable]");

            // Update variable index.
            var index = variables.Item1.FindIndex(item => item == (Variable)m_VariableProp.objectReferenceValue);

            index = EditorGUILayout.Popup(new GUIContent("Variable", "The variable to modify."), index, variables.Item2.ToArray());

            if (index > -1)
            {
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

            EditorGUI.EndDisabledGroup();
        }

        public override void OnSceneGUI()
        {
            base.OnSceneGUI();

            if (Event.current.type == EventType.Repaint)
            {
                if (m_EnemyController && m_EnemyController.IsPlacedOnBrick())
                {
                    var spawnPositionWorld = m_EnemyController.GetBrickCenter() + m_EnemyController.GetBrickRotation() * m_SpawnAreaCenterProp.vector3Value;
                    var worldToLocalRotation = Quaternion.Inverse(m_EnemyController.GetBrickRotation());

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
                    Handles.matrix = Matrix4x4.TRS(spawnPositionWorld, m_EnemyController.GetBrickRotation(), Vector3.one);
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
