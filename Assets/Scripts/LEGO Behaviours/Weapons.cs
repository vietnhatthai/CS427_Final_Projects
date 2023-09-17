using System.Collections.Generic;
using Unity.LEGO.Game;
using Unity.LEGO.UI;
using UnityEngine;


namespace Unity.LEGO.Behaviours.Actions
{
    public class Weapons : SpawnAction
    {
        [SerializeField, Tooltip("The variable to modify.")]
        Game.Variable m_Variable = null;

        [SerializeField, Tooltip("The weapon data to use for the spawned weapon.")]
        List<WeaponData> m_WeaponData = default;

        [SerializeField, Tooltip("The UI panel containing the layoutGroup for displaying objectives.")]
        WeaponHUB m_ObjectiveWeapons = default;

        private bool m_chooseWeapon = true;
        private int m_WeaponIndex = 0;

        protected override void Start()
        {
            base.Start();
            m_Repeat = false;
        }

        private void SelectWeaponHandler(int index)
        {
            Debug.Log("SelectWeaponHandler: " + index);
            m_WeaponIndex = index;
            m_ObjectiveWeapons.Hide();
            m_Active = true;
            ChangeModel(m_WeaponData[m_WeaponIndex].m_Prefab);
            VariableManager.SetValue(m_Variable, VariableManager.GetValue(m_Variable) - (int)m_WeaponData[m_WeaponIndex].m_Price);
            m_chooseWeapon = false;
        }

        protected void Update()
        {
            if (m_Active && m_chooseWeapon)
            {
                m_chooseWeapon = true;
                m_ObjectiveWeapons.Show();
                for (int i = 0; i < m_WeaponData.Count; i++)
                {
                    m_ObjectiveWeapons.AddWeapon(m_WeaponData[i].m_Icon, i);
                    m_ObjectiveWeapons.SelectWeapon += SelectWeaponHandler;
                }
                m_Active = false;
            }

            if (!m_chooseWeapon && m_Active)
            {
                base.Update();
            }

            if (!m_Active && !m_chooseWeapon)
            {
                m_chooseWeapon = true;
            }
        }
    }
}