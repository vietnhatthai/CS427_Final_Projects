using System.IO;
using Unity.LEGO.Behaviours.Actions;
using Unity.LEGO.Game;
using UnityEditor;
using UnityEngine;

namespace Unity.LEGO.EditorExt
{
    [CustomEditor(typeof(EnemyMoveAction), true)]
    public class EnemyMoveActionEditor : MovementActionEditor
    {
        SerializedProperty m_SpeedProp;
        Editor m_VariableEditor;

        SerializedProperty m_MinSpeedProp;
        SerializedProperty m_MaxSpeedProp;
        SerializedProperty m_IdleSpeedProp;
        SerializedProperty m_RotationSpeedProp;
        SerializedProperty m_JumpSpeedProp;
        SerializedProperty m_MaxJumpsInAirProp;
        SerializedProperty m_GravityProp;
        SerializedProperty m_VariableProp;
        SerializedProperty m_PowerProp;
        SerializedProperty m_RemoveBricksProp;

        protected override void OnEnable()
        {
            base.OnEnable();
            m_SpeedProp = serializedObject.FindProperty("m_Speed");
            m_MinSpeedProp = serializedObject.FindProperty("m_MinSpeed");
            m_MaxSpeedProp = serializedObject.FindProperty("m_MaxSpeed");
            m_IdleSpeedProp = serializedObject.FindProperty("m_IdleSpeed");
            m_RotationSpeedProp = serializedObject.FindProperty("m_RotationSpeed");
            m_JumpSpeedProp = serializedObject.FindProperty("m_JumpSpeed");
            m_MaxJumpsInAirProp = serializedObject.FindProperty("m_MaxJumpsInAir");
            m_GravityProp = serializedObject.FindProperty("m_Gravity");
            m_VariableProp = serializedObject.FindProperty("m_Variable");
            m_PowerProp = serializedObject.FindProperty("m_Power");
            m_RemoveBricksProp = serializedObject.FindProperty("m_RemoveBricks");
        }

        protected override void CreateGUI()
        {
            EditorGUILayout.PropertyField(m_SpeedProp);
            EditorGUILayout.PropertyField(m_PowerProp);
            EditorGUILayout.PropertyField(m_RemoveBricksProp);
            EditorGUILayout.PropertyField(m_MinSpeedProp);
            EditorGUILayout.PropertyField(m_MaxSpeedProp);
            EditorGUILayout.PropertyField(m_IdleSpeedProp);
            EditorGUILayout.PropertyField(m_RotationSpeedProp);
            EditorGUILayout.PropertyField(m_JumpSpeedProp);
            EditorGUILayout.PropertyField(m_MaxJumpsInAirProp);
            EditorGUILayout.PropertyField(m_GravityProp);

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
    }
}