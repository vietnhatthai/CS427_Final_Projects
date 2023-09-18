using Unity.LEGO.Behaviours.Actions;
using UnityEditor;

namespace Unity.LEGO.EditorExt
{
    [CustomEditor(typeof(EnemyMoveAction), true)]
    public class EnemyMoveActionEditor : MovementActionEditor
    {
        SerializedProperty m_SpeedProp;

        // m_MinSpeed
        // m_MaxSpeed
        // m_IdleSpeed
        // m_RotationSpeed
        // m_JumpSpeed
        // m_MaxJumpsInAir
        // m_Gravity

        SerializedProperty m_MinSpeedProp;
        SerializedProperty m_MaxSpeedProp;
        SerializedProperty m_IdleSpeedProp;
        SerializedProperty m_RotationSpeedProp;
        SerializedProperty m_JumpSpeedProp;
        SerializedProperty m_MaxJumpsInAirProp;
        SerializedProperty m_GravityProp;

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
        }

        protected override void CreateGUI()
        {
            EditorGUILayout.PropertyField(m_SpeedProp);

            EditorGUILayout.PropertyField(m_MinSpeedProp);
            EditorGUILayout.PropertyField(m_MaxSpeedProp);
            EditorGUILayout.PropertyField(m_IdleSpeedProp);
            EditorGUILayout.PropertyField(m_RotationSpeedProp);
            EditorGUILayout.PropertyField(m_JumpSpeedProp);
            EditorGUILayout.PropertyField(m_MaxJumpsInAirProp);
            EditorGUILayout.PropertyField(m_GravityProp);
        }
    }
}