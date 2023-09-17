using LEGOModelImporter;
using Unity.LEGO.Behaviours.Actions;
using UnityEditor;
using UnityEngine;
using Unity.LEGO.Game;
using System.IO;
using UnityEditorInternal;

namespace Unity.LEGO.EditorExt
{
    [CustomEditor(typeof(Weapons), true)]
    public class WeaponEditor : RepeatableActionEditor
    {
        static GUIStyle m_LabelStyle;

        Weapons m_Weapons;
        Editor m_VariableEditor;

        SerializedProperty m_WeaponsDataProp;
        SerializedProperty m_VariableProp;
        SerializedProperty m_SpeechBubbleInfoProp;
        SerializedProperty m_SpeechBubblePromptPrefabProp;

        protected override void OnEnable()
        {
            base.OnEnable();

            m_Weapons = (Weapons)m_Action;

            m_WeaponsDataProp = serializedObject.FindProperty("m_WeaponsData");
            m_VariableProp = serializedObject.FindProperty("m_Variable");
            m_SpeechBubbleInfoProp = serializedObject.FindProperty("m_SpeechBubbleInfo");
            m_SpeechBubblePromptPrefabProp = serializedObject.FindProperty("m_SpeechBubblePromptPrefab");
        }

        protected override void CreateGUI()
        {
            EditorGUI.BeginDisabledGroup(EditorApplication.isPlaying);

            EditorGUILayout.PropertyField(m_WeaponsDataProp);
            EditorGUILayout.PropertyField(m_SpeechBubbleInfoProp);
            EditorGUILayout.PropertyField(m_SpeechBubblePromptPrefabProp);

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
