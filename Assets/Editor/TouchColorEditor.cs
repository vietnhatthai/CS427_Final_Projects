using UnityEditor;
using UnityEngine;
using Unity.LEGO.Behaviours.Triggers;

namespace Unity.LEGO.EditorExt
{
    [CustomEditor(typeof(TouchColor), true)]
    public class TouchColorEditor : SensoryTriggerEditor
    {
        protected SerializedProperty m_MouldingColourProp;

        protected override void OnEnable()
        {
            base.OnEnable();

            m_MouldingColourProp = serializedObject.FindProperty("m_MouldingColour");
        }

        protected override void CreateGUI()
        {
            EditorGUI.BeginDisabledGroup(EditorApplication.isPlaying);

            EditorGUILayout.PropertyField(m_MouldingColourProp, new GUIContent("Moulding Colour 0", "The colour to change to when triggered."));

            EditorGUILayout.PropertyField(m_ScopeProp);
            CreateTargetGUI();
            EditorGUILayout.PropertyField(m_SenseProp);
            if ((SensoryTrigger.Sense)m_SenseProp.enumValueIndex == SensoryTrigger.Sense.Tag)
            {
                m_SenseTagProp.stringValue = EditorGUILayout.TagField(new GUIContent("Tag", "The tag to sense."), m_SenseTagProp.stringValue);
            }

            EditorGUI.EndDisabledGroup();

            EditorGUILayout.PropertyField(m_RepeatProp);

            CreateConditionsGUI();
        }
    }
}
