using UnityEditor;
using UnityEngine;
using Unity.LEGO.Behaviours.Actions;
using Unity.LEGO.EditorExt;

namespace Lego.SummerJam.NoFrogsAllowed.Editor
{
    [CustomEditor(typeof(CustomTrigger), true)]
    public class CustomTriggerEditor : TriggerEditor
    {
        protected override void CreateGUI()
        {
            CreateTargetGUI();

            //EditorGUILayout.PropertyField(m_RepeatProp);

            //GUILayout.Label("Conditions", EditorStyles.boldLabel);

            //CreateConditionsGUI(false);
        }
    }
}
