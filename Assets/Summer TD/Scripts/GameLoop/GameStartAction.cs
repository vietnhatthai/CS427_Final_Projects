using System;
using System.Collections.Generic;
using UnityEngine;

namespace Lego.SummerJam.NoFrogsAllowed
{
    //[RequireComponent(typeof(CustomAction))]
    public class GameStartAction : MonoBehaviour, IAction
    {
        public static Action OnGameStart;
        public static Action<TurretSpawner> OnSelectCannon; 

        [SerializeField] private TurretSpawner _turretSpawner;

        public void Activate()
        {
            OnSelectCannon?.Invoke(_turretSpawner);
            OnGameStart?.Invoke();
        }
    }
}
