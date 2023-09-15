using UnityEngine;
using Unity.LEGO.Behaviours.Actions;
using Unity.LEGO.Behaviours.Triggers;

namespace Lego.SummerJam.NoFrogsAllowed
{
    public class CustomTrigger : Trigger
    {
        public void TriggerActions()
        {
            foreach (Action action in m_SpecificTargetActions)
            {
                action.Activate();
            }
        }
    }
}
