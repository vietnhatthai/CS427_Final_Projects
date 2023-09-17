using UnityEditor;
using UnityEngine;
using Unity.LEGO.Behaviours.Triggers;
using System.Collections.Generic;
using LEGOMaterials;

namespace Unity.LEGO.EditorExt
{
    [CustomEditor(typeof(TouchColor), true)]
    public class TouchColorEditor : SensoryTriggerEditor
    {
        static readonly float alphaBarHeight = 3.0f;
        protected SerializedProperty m_MouldingColourProp;

        protected override void OnEnable()
        {
            base.OnEnable();

            m_MouldingColourProp = serializedObject.FindProperty("m_MouldingColour");
        }

        protected override void CreateGUI()
        {

            serializedObject.Update();

            EditorGUI.BeginDisabledGroup(true);

            EditorGUILayout.PropertyField(m_MouldingColourProp);

            EditorGUI.EndDisabledGroup();

            if (m_MouldingColourProp != null)
            {  
                DrawMouldingColour(m_MouldingColourProp.intValue, m_MouldingColourProp.intValue, false);
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawMouldingColour(int colourID, int listIndex, bool multipleValues)
        {
            var position = EditorGUILayout.GetControlRect();

            // Draw label
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), new GUIContent("Moulding Colour " + listIndex));

            if (multipleValues)
            {
                // Create box and tooltip.
                GUI.Box(position, new GUIContent("", "Multiple colours selected"));
                var colorRect = new Rect(position.x + 1.0f, position.y + 1.0f, position.width - 2.0f, position.height - 2.0f);
                var lineRect = new Rect(position.x + 1.0f, position.y + 1.0f + colorRect.height / 2.0f, 10.0f, 1.0f);
                EditorGUI.DrawRect(colorRect, new Color32(209, 209, 209, 255));
                EditorGUI.DrawRect(lineRect, Color.grey);
            }
            else
            {
                var mouldingColourID = (MouldingColour.Id)colourID;
                var colour = MouldingColour.GetColour(mouldingColourID);

                // Create box and tooltip.
                GUI.Box(position, new GUIContent("", ObjectNames.NicifyVariableName((int)mouldingColourID + " - " + mouldingColourID.ToString())));

                // Draw rects with colour.
                var colorRect = new Rect(position.x + 1.0f, position.y + 1.0f, position.width - 2.0f, position.height - 2.0f - alphaBarHeight);
                var alphaRect = new Rect(position.x + 1.0f, position.y + 1.0f + colorRect.height, Mathf.Round((position.width - 2.0f) * colour.a), alphaBarHeight);
                var blackRect = new Rect(position.x + 1.0f + alphaRect.width, position.y + 1.0f + colorRect.height, position.width - 2.0f - alphaRect.width, alphaBarHeight);
                EditorGUI.DrawRect(colorRect, new Color(colour.r, colour.g, colour.b));
                EditorGUI.DrawRect(alphaRect, Color.white);
                EditorGUI.DrawRect(blackRect, Color.black);
            }

            // Detect click.
            if (Event.current.type == EventType.MouseDown)
            {
                if (position.Contains(Event.current.mousePosition))
                {
                    MouldingColourPicker.Show((c) =>
                    {
                        m_MouldingColourProp.intValue = (int)MouldingColour.GetId(c);
                        
                       serializedObject.ApplyModifiedProperties();

                    }, false, false, true);
                }
            }
        }
    }
}
