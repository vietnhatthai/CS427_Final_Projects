using System.Collections.Generic;
using Unity.LEGO.Behaviours.Triggers;
using Unity.LEGO.Game;
using Unity.LEGO.UI;
using UnityEngine;
using static Unity.LEGO.UI.SpeechBubblePrompt;

namespace Unity.LEGO.Behaviours.Actions
{
    public class Weapons : Action
    {
        public const int MaxCharactersPerSpeechBubble = 60;

        [SerializeField, Tooltip("Weapons data.")]
        private ConfigWeapons m_WeaponsData;

        [SerializeField, Tooltip("The variable to modify.")]
        private Game.Variable m_Variable;

        // setting rotation
        [SerializeField, Tooltip("The rotation.")]
        private Vector3 m_Rotation;

        [SerializeField]
        BubbleInfo m_SpeechBubbleInfo = new BubbleInfo
        {
            Text = "Hello!",
            Type = Type.InformationSign
        };

        [SerializeField]
        GameObject m_SpeechBubblePromptPrefab = default;

        SpeechBubblePrompt m_SpeechBubblePrompt;
        bool m_PromptActive = true;
        int m_Id;

        private GameObject m_CurrentObject;
        private GameObject m_ObjectToSpawn;
        private GameObject instant;

        private WeaponHUB weaponHUB;

        protected override void Reset()
        {
            base.Reset();

            m_IconPath = "Assets/LEGO/Gizmos/LEGO Behaviour Icons/Weapons.png";
        }

        protected override void Start()
        {
            base.Start();
            m_CurrentObject = gameObject;
            m_ObjectToSpawn = m_WeaponsData.GetWeapon(0).m_Prefab;

            weaponHUB = FindObjectOfType<WeaponHUB>();
        }

        private void SelectWeaponHandler(int index)
        {
            m_ObjectToSpawn = m_WeaponsData.GetWeapon(index).m_Prefab;

            if (instant == null)
            {

                int price = (int)m_WeaponsData.GetWeapon(index).m_Price;
                int remaining = VariableManager.GetValue(m_Variable) - price;
                if (remaining < 0)
                {
                    if (!m_SpeechBubblePrompt)
                        SetupPrompt();
                    UpdatePrompt(IsVisible());
                    weaponHUB.Hide();
                    return;
                }

                VariableManager.SetValue(m_Variable, remaining);
                instant = Instantiate(m_ObjectToSpawn, m_CurrentObject.transform.position, m_CurrentObject.transform.rotation);

                // Rotate the object relative to the parent
                instant.transform.rotation = m_CurrentObject.transform.rotation * Quaternion.Euler(m_Rotation);

                m_CurrentObject.GetComponent<InputTrigger>().m_OtherKey = InputTrigger.Key.Q;
                m_CurrentObject.GetComponent<InputTrigger>().UpdatePrompt();
            }
            weaponHUB.Hide();
        }


        protected void Update()
        {
            if (m_Active && weaponHUB != null)
            {
                if (instant == null)
                {
                    weaponHUB.Show();
                    for (int i = 0; i < m_WeaponsData.weapons.Length; i++)
                    {
                        weaponHUB.AddWeapon(m_WeaponsData.GetWeapon(i).m_Icon, i);
                    }
                    weaponHUB.SelectWeapon += SelectWeaponHandler;
                }
                else
                {
                    Destroy(instant);
                    m_CurrentObject.GetComponent<InputTrigger>().m_OtherKey = InputTrigger.Key.E;
                    m_CurrentObject.GetComponent<InputTrigger>().UpdatePrompt();
                    int sell = (int)m_WeaponsData.GetWeapon(0).m_SellPrice;
                    VariableManager.SetValue(m_Variable, VariableManager.GetValue(m_Variable) + sell);
                }

                m_Active = false;
            }
        }

        void SetupPrompt()
        {
            PromptPlacementHandler promptHandler = null;

            //foreach (var brick in m_ScopedBricks)
            //{
            //    if (brick.GetComponent<PromptPlacementHandler>())
            //    {
            //        promptHandler = brick.GetComponent<PromptPlacementHandler>();
            //    }

            //    var speakActions = brick.GetComponents<Weapons>();

            //    foreach (var speakAction in speakActions)
            //    {
            //        if (speakAction.m_SpeechBubblePrompt)
            //        {
            //            m_SpeechBubblePrompt = speakAction.m_SpeechBubblePrompt;
            //            break;
            //        }
            //    }
            //}

            var activeFromStart = IsVisible();

            // Create a new speech bubble prompt if none was found.
            if (!m_SpeechBubblePrompt)
            {
                if (!promptHandler)
                {
                    promptHandler = gameObject.AddComponent<PromptPlacementHandler>();
                }

                var go = Instantiate(m_SpeechBubblePromptPrefab, promptHandler.transform);
                m_SpeechBubblePrompt = go.GetComponent<SpeechBubblePrompt>();

                // Get the current scoped bounds - might be different than the initial scoped bounds.
                var scopedBounds = GetScopedBounds(m_ScopedBricks, out _, out _);
                promptHandler.AddInstance(go, scopedBounds, PromptPlacementHandler.PromptType.SpeechBubble, activeFromStart);
            }

            // Add this Speak Action to the speech bubble prompt.
            List<BubbleInfo> m_SpeechBubbleInfos = new List<BubbleInfo>() { m_SpeechBubbleInfo };
            Debug.Log("m_SpeechBubbleInfos.Count " + m_SpeechBubbleInfos.Count);
            Debug.Log("m_SpeechBubblePrompt " + m_SpeechBubblePrompt);
            m_Id = m_SpeechBubblePrompt.AddSpeech(m_SpeechBubbleInfos, 1, false, SpeechFinished, activeFromStart, promptHandler);
        }

        void SpeechFinished(int id)
        {
            if (m_Id == id)
            {
                UpdatePrompt(false);
            }
        }

        void UpdatePrompt(bool active)
        {
            if (m_PromptActive != active)
            {
                m_PromptActive = active;

                if (active)
                {
                    m_SpeechBubblePrompt.Activate(m_Id);
                }
                else
                {
                    m_SpeechBubblePrompt.Deactivate(m_Id);
                }
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawRay(transform.position, transform.rotation * Quaternion.Euler(m_Rotation) * Vector3.forward * 2);
            Gizmos.color = Color.green;
            Gizmos.DrawRay(transform.position, transform.rotation * Quaternion.Euler(m_Rotation) * Vector3.up * 2);
            Gizmos.color = Color.blue;
            Gizmos.DrawRay(transform.position, transform.rotation * Quaternion.Euler(m_Rotation) * Vector3.right * 2);
        }
    }
}