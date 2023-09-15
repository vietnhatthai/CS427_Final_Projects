using UnityEngine;
using Unity.LEGO.Behaviours.Actions;

namespace Lego.SummerJam.NoFrogsAllowed
{
    public class CustomAction : RepeatableAction
    {
        [SerializeField] private GameObject _actionObject;

        public override void Activate()
        {
            base.Activate();
            IAction action = _actionObject.GetComponent<IAction>();
            action.Activate();
        }
    }
}
