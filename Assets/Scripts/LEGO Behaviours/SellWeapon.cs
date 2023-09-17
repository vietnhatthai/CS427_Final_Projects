using UnityEngine;

namespace Unity.LEGO.Behaviours.Actions
{
    public class SellWeapon : Action
    {
        // previous object
        public GameObject m_PreviousObject;

        protected override void Start()
        {
            base.Start();
            m_PreviousObject = gameObject;
        }

        protected void Update()
        {
            if (m_Active)
            {
                m_PreviousObject.SetActive(true);
                Destroy(gameObject);
                m_Active = false;
            }
        }

    }
}