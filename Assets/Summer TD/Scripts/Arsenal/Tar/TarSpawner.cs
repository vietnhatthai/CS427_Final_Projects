using System;
using UnityEngine;
using Unity.LEGO.Game;

namespace Lego.SummerJam.NoFrogsAllowed
{
    [RequireComponent(typeof(CustomAction))]
    public class TarSpawner : MonoBehaviour, IAction
    {
        public static Action OnBuyTrap;

        #region Serialized Fields
        [SerializeField] private int _id;

        [Space(8)]
        [SerializeField] private GameObject _buyerObj;
        [SerializeField] private GameObject _sellerObj;
        [SerializeField] private GameObject _trapGroundObj;
        [SerializeField] private GameObject _tarPrefab;

        [Space(8)]
        [SerializeField] private int _price;

        [Space(10)]
        // Note: The following 'Variable(s)' was created using LEGO Microgame Editors
        // It's a ScriptableObject located at Assets/LEGO/Scriptable Objects
        [SerializeField] private Variable _coins;
        #endregion

        private GameObject _tarObj;
        private GameProgressData _gameProgress;
        private TrapDataModel _data;

        #region Unity Messages
        private void OnEnable()
        {
            GameLoopController.OnChangeGameState += OnGameStateChange;
        }

        private void OnDisable()
        {
            GameLoopController.OnChangeGameState -= OnGameStateChange;
        }

        private void Awake()
        {
            _data = new TrapDataModel
            { 
                ID = _id,
                Type = TrapType.Tar
            };
            _gameProgress = AssetResources.GameProgress;
        }

        #endregion

        public void HideAll()
        {
            _buyerObj.SetActive(false);
            _sellerObj.SetActive(false);
        }

        public void ShowSeller()
        {
            _sellerObj.SetActive(true);
            _buyerObj.SetActive(false);
            _trapGroundObj.SetActive(true);
            if (_tarObj != null)
            {
                Destroy(_tarObj);
            }
        }

        public void ShowTar()
        {
            _sellerObj.SetActive(false);
            _trapGroundObj.SetActive(false);
            _buyerObj.SetActive(true);
            _tarObj = Instantiate(_tarPrefab, transform);
            _tarObj.SetActive(true);
        }

        private void BuyTrap()
        {
            int currentCoins = VariableManager.GetValue(_coins);
            if (currentCoins - _price < 0)
            {
                return;
            }

            _gameProgress.Data.TrapList.Add(_data);
            VariableManager.SetValue(_coins, currentCoins - _price);
            ShowTar();
            OnBuyTrap?.Invoke();
        }

        private void SellTrap()
        {
            _gameProgress.Data.TrapList.Remove(_data);
            int currentCoins = VariableManager.GetValue(_coins);
            VariableManager.SetValue(_coins, currentCoins + _price);
            ShowSeller();
        }

        public void Activate()
        {
            if (_tarObj != null)
            {
                SellTrap();
            }
            else
            {
                BuyTrap();
            }
        }

        private void OnGameStateChange(GameState currentGameState)
        {
            switch (currentGameState)
            {
                //case GameState.BuildMode:
                //    {
                //        _buyerObj.SetActive(_spikeTrapObj != null);
                //        break;
                //    }

                case GameState.LevelIntro:
                    {
                        _buyerObj.SetActive(false);
                        _sellerObj.SetActive(false);
                        break;
                    }

                default:
                    break;
            }
        }
    }
}
