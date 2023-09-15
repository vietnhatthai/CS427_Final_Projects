using UnityEditor;
using UnityEngine;
using Unity.LEGO.Behaviours.Actions;
using Unity.LEGO.EditorExt;

namespace Lego.SummerJam.NoFrogsAllowed.Editor
{
    [CustomEditor(typeof(CustomAction), true)]
    public class CustomActionEditor : RepeatableActionEditor
    {
        SerializedProperty _actionObject;

        protected override void OnEnable()
        {
            base.OnEnable();
            _actionObject = serializedObject.FindProperty("_actionObject");
        }

        protected override void CreateGUI()
        {
            //EditorGUILayout.PropertyField(m_AudioProp);
            //EditorGUILayout.PropertyField(m_AudioVolumeProp);
            EditorGUILayout.PropertyField(_actionObject);
        }

        public override void OnSceneGUI()
        {
            base.OnSceneGUI();
        }
    }
}
