using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lego.SummerJam.NoFrogsAllowed
{
    public class GatesController : MonoBehaviour
    {
        [SerializeField] private GameObject _staticGates;
        [SerializeField] private GameObject _explodingGatesPrefab;

        private GameObject _explodingGates;

        private void OnEnable()
        {
            GameLoopController.OnChangeGameState += OnGameStateChange;
        }

        private void OnDisable()
        {
            GameLoopController.OnChangeGameState -= OnGameStateChange;
        }

        private void OnGameStateChange(GameState currentGameState)
        {
            switch (currentGameState)
            {
                case GameState.GateExplosion:
                    {
                        _staticGates.SetActive(false);
                        _explodingGates = Instantiate(_explodingGatesPrefab, transform);
                        _explodingGates.SetActive(true);
                        _explodingGates.transform.localPosition = Vector3.zero;
                        StartCoroutine(CleanUpRoutine());
                        break;
                    }

                default:
                    break;
            }
        }

        private IEnumerator CleanUpRoutine()
        {
            yield return new WaitForSeconds(4.0f);

            for (int idx = 0; idx < 3; ++idx)
            {
                _explodingGates.SetActive(false);
                yield return new WaitForSeconds(0.08f);
                _explodingGates.SetActive(true);
                yield return new WaitForSeconds(0.2f);
            }

            Destroy(_explodingGates);
        }
    }
}
