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
        SerializedProperty m_PowerProp;
        SerializedProperty m_RemoveBricksProp;
        SerializedProperty m_MinusVariableProp;
        SerializedProperty m_BonusVariableProp;

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
            m_PowerProp = serializedObject.FindProperty("m_Power");
            m_RemoveBricksProp = serializedObject.FindProperty("m_RemoveBricks");
            m_MinusVariableProp = serializedObject.FindProperty("m_MinusVariable");
            m_BonusVariableProp = serializedObject.FindProperty("m_BonusVariable");
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

            if (m_MinusVariableProp == null)
            {
                EditorGUI.EndDisabledGroup();
                return;
            }

            // Refresh variable list.
            var minusVariables = GetAvailableVariables();
            minusVariables.Item2.Add("[Add New Variable]");

            // Update variable index.
            var minusIndex = minusVariables.Item1.FindIndex(item => item == (Variable)m_MinusVariableProp.objectReferenceValue);

            minusIndex = EditorGUILayout.Popup(new GUIContent("Variable", "The variable to modify."), minusIndex, minusVariables.Item2.ToArray());

            if (minusIndex > -1)
            {
                DrawSeparator();
                EditorGUILayout.LabelField("Variable Settings", EditorStyles.boldLabel);

                if (minusIndex == minusVariables.Item2.Count - 1)
                {
                    var newVariable = CreateInstance<Variable>();
                    newVariable.Name = "Variable (Minus)";
                    var newVariableAssetPath = AssetDatabase.GenerateUniqueAssetPath(Path.Combine(VariableManager.k_VariablePath, "Variable.asset"));
                    AssetDatabase.CreateAsset(newVariable, newVariableAssetPath);
                    m_MinusVariableProp.objectReferenceValue = newVariable;
                }
                else
                {
                    m_MinusVariableProp.objectReferenceValue = minusVariables.Item1[minusIndex];

                    // Only recreate editor if necessary.
                    if (!m_VariableEditor || m_VariableEditor.target != m_MinusVariableProp.objectReferenceValue)
                    {
                        DestroyImmediate(m_VariableEditor);
                        m_VariableEditor = CreateEditor(m_MinusVariableProp.objectReferenceValue);
                    }

                    m_VariableEditor.OnInspectorGUI();

                    if (GUILayout.Button("Delete Variable"))
                    {
                        AssetDatabase.DeleteAsset(minusVariables.Item3[minusIndex]);
                    }
                }
            }

            EditorGUI.EndDisabledGroup();

            EditorGUI.BeginDisabledGroup(EditorApplication.isPlaying);

            if (m_BonusVariableProp == null)
            {
                EditorGUI.EndDisabledGroup();
                return;
            }

            // Refresh variable list.
            var bonusVariables = GetAvailableVariables();
            bonusVariables.Item2.Add("[Add New Variable]");

            // Update variable index.
            var bonusIndex = bonusVariables.Item1.FindIndex(item => item == (Variable)m_BonusVariableProp.objectReferenceValue);

            bonusIndex = EditorGUILayout.Popup(new GUIContent("Variable", "The variable to modify."), bonusIndex, bonusVariables.Item2.ToArray());

            if (bonusIndex > -1)
            {
                DrawSeparator();
                EditorGUILayout.LabelField("Variable Settings", EditorStyles.boldLabel);

                if (bonusIndex == bonusVariables.Item2.Count - 1)
                {
                    var newVariable = CreateInstance<Variable>();
                    newVariable.Name = "Variable (Bonus)";
                    var newVariableAssetPath = AssetDatabase.GenerateUniqueAssetPath(Path.Combine(VariableManager.k_VariablePath, "Variable.asset"));
                    AssetDatabase.CreateAsset(newVariable, newVariableAssetPath);
                    m_BonusVariableProp.objectReferenceValue = newVariable;
                }
                else
                {
                    m_BonusVariableProp.objectReferenceValue = bonusVariables.Item1[bonusIndex];

                    // Only recreate editor if necessary.
                    if (!m_VariableEditor || m_VariableEditor.target != m_BonusVariableProp.objectReferenceValue)
                    {
                        DestroyImmediate(m_VariableEditor);
                        m_VariableEditor = CreateEditor(m_BonusVariableProp.objectReferenceValue);
                    }

                    m_VariableEditor.OnInspectorGUI();

                    if (GUILayout.Button("Delete Variable"))
                    {
                        AssetDatabase.DeleteAsset(bonusVariables.Item3[bonusIndex]);
                    }
                }
            }

            EditorGUI.EndDisabledGroup();

        }
    }
}