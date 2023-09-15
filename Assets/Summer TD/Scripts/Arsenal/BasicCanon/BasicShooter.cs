using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lego.SummerJam.NoFrogsAllowed
{
    public class BasicShooter : MonoBehaviour
    {
        [SerializeField] private GameObject _activateGuideObj;

        private void OnEnable()
        {
            GameLoopController.OnChangeGameState += OnChangeGameState;
        }

        private void OnDisable()
        {
            GameLoopController.OnChangeGameState -= OnChangeGameState;
        }

        #region System.Action Handlers
        private void OnChangeGameState(GameState currentGameState)
        {
            switch (currentGameState)
            {
                case GameState.ShootMode:
                    _activateGuideObj.SetActive(false);
                    break;

                default:
                    break;
            }
        }
        #endregion

        private void OnReleaseFrogs()
        {
            _activateGuideObj.SetActive(false);
        }
    }
}
