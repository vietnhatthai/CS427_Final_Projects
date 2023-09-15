using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lego.SummerJam.NoFrogsAllowed
{
    public class StoryManager : MonoBehaviour
    {
        [SerializeField] private GameObject _welcomeMessageObj;
        [SerializeField] private GameObject _startGameMessageObj;
        [SerializeField] private GameObject _trapsUnlockMessageObj;
        [SerializeField] private GameObject _havocMessageObj;

        [SerializeField] private List<GameObject> _defaultMessageObjs;

        private GameProgressData _gameProgress;

        private void OnEnable()
        {
            TurretSpawner.OnBuyCannon += OnBuyCannon;
            TarSpawner.OnBuyTrap += OnBuyTrap;
            SpikeTrapSpawner.OnBuyTrap += OnBuyTrap;
        }

        private void OnDisable()
        {
            TurretSpawner.OnBuyCannon -= OnBuyCannon;
            TarSpawner.OnBuyTrap -= OnBuyTrap;
            SpikeTrapSpawner.OnBuyTrap -= OnBuyTrap;
        }

        private void Start()
        {
            _welcomeMessageObj.SetActive(false);
            _startGameMessageObj.SetActive(false);
            _trapsUnlockMessageObj.SetActive(false);
            _havocMessageObj.SetActive(false);

            foreach (GameObject defaultMessageObj in _defaultMessageObjs)
            {
                defaultMessageObj.SetActive(false);
            }

            _gameProgress = AssetResources.GameProgress;
            switch(_gameProgress.Data.Level)
            {
                case 0:
                    _welcomeMessageObj.SetActive(true);
                    break;

                case 1:
                    _trapsUnlockMessageObj.SetActive(true);
                    break;

                default:
                    GameObject defaultMsgObj = _defaultMessageObjs[Random.Range(0, _defaultMessageObjs.Count)];
                    defaultMsgObj.SetActive(true);
                    break;
            }
        }

        private void OnBuyCannon()
        {
            if (_gameProgress.Data.Level != 0)
            {
                return;
            }

            _welcomeMessageObj.SetActive(false);
            _startGameMessageObj.SetActive(true);
        }

        private void OnBuyTrap()
        {
            if (_gameProgress.Data.Level != 1)
            {
                return;
            }

            _trapsUnlockMessageObj.SetActive(false);
            _havocMessageObj.SetActive(true);
        }
    }
}
