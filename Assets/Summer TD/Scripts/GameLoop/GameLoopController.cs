using System;
using System.Collections;
using UnityEngine;
using Unity.LEGO.Minifig;
using Unity.LEGO.Game;

namespace Lego.SummerJam.NoFrogsAllowed
{
    public enum GameState
    {
        BuildMode,
        LevelIntro,
        GateExplosion,
        ShootMode
    }

    public class GameLoopController : MonoBehaviour
    {
        public static Action<GameState> OnChangeGameState;
        public static Action<Transform> OnSetPlayerTransform;
        //public static Action OnReleaseFrogs;

        #region Serialized Fields
        [SerializeField] private MinifigController _minifigController;
        [SerializeField] private int _coinAmt;
        [SerializeField] private int _level;

        [Space(10)]
        // Note: The following 'Variable(s)' was created using LEGO Microgame Editors
        // It's a ScriptableObject located at Assets/LEGO/Scriptable Objects
        [SerializeField] private Variable _coins;
        #endregion

        private GameState _currentGameState;
        private GameProgressData _gameProgress;

        private void OnEnable()
        {
            EventManager.AddListener<OptionsMenuEvent>(OnGamePause);
            EventManager.AddListener<GameOverEvent>(OnGameOver);

            GameStartAction.OnGameStart += NextState;
            GameStartAction.OnSelectCannon += OnSelectTurret;

            CameraDirector.OnExplodeGates += NextState;
            CameraDirector.OnLevelIntroDone += NextState;
        }

        private void OnDisable()
        {
            EventManager.RemoveListener<OptionsMenuEvent>(OnGamePause);
            EventManager.RemoveListener<GameOverEvent>(OnGameOver);

            GameStartAction.OnGameStart -= NextState;
            GameStartAction.OnSelectCannon -= OnSelectTurret;

            CameraDirector.OnExplodeGates -= NextState;
            CameraDirector.OnLevelIntroDone -= NextState;

            _gameProgress.Data.Money = VariableManager.GetValue(_coins);
            _gameProgress.SaveData();
        }

        private void Awake()
        {
            Application.targetFrameRate = -1;
            _gameProgress = AssetResources.GameProgress;
            //_gameProgress.LoadData();
            _gameProgress.LoadDefaults();
        }

        private void Start()
        {
            ChangeToBuildMode();
            StartCoroutine(LoadGameDataRoutine());
        }

        private IEnumerator LoadGameDataRoutine()
        {
            _gameProgress.Data.Level = _level;
            _gameProgress.Data.Money = _coinAmt;

            yield return new WaitForEndOfFrame();
            VariableManager.SetValue(_coins, _gameProgress.Data.Money);
        }

        #region Event Handlers
        private void NextState()
        {
            switch (_currentGameState)
            {
                case GameState.BuildMode:
                    {
                        //ChangeToShootMode();
                        ChangeToLevelIntro();
                        break;
                    }
                case GameState.LevelIntro:
                    {
                        _currentGameState = GameState.GateExplosion;
                        break;
                    }
                case GameState.GateExplosion:
                    {
                        ChangeToShootMode();
                        break;
                    }
                case GameState.ShootMode:
                    {
                        OnSetPlayerTransform?.Invoke(_minifigController.transform);
                        break;
                    }
                default:
                    break;
            }

            Debug.Log("current state: " + _currentGameState.ToString());
            OnChangeGameState?.Invoke(_currentGameState);
        }

        public void OnGamePause(OptionsMenuEvent evt)
        {
            Cursor.visible = evt.Active;
            if (evt.Active)
            {
                Cursor.lockState = CursorLockMode.Confined;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
            }
        }

        private void OnSelectTurret(TurretSpawner turretSpawner)
        {
            Debug.Log("on select turret");
            turretSpawner.SetPlayer(_minifigController.transform);
        }
        #endregion

        #region Game State Handlers
        private void ChangeToBuildMode()
        {
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
            _currentGameState = GameState.BuildMode;
            _minifigController.SetInputEnabled(true);
        }

        private void ChangeToLevelIntro()
        {
            //OnReleaseFrogs?.Invoke();
            _currentGameState = GameState.LevelIntro;
            _minifigController.SetInputEnabled(false);
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void ChangeToShootMode()
        {
            //OnReleaseFrogs?.Invoke();
            _currentGameState = GameState.ShootMode;
            _minifigController.SetInputEnabled(false);
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void OnGameOver(GameOverEvent evt)
        {
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
            _gameProgress.Data.Win = evt.Win;
        }
        #endregion
    }
}
