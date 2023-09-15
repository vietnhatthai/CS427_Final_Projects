using System;
using System.Collections;
using UnityEngine;
using Cinemachine;

namespace Lego.SummerJam.NoFrogsAllowed
{
    public class CameraDirector : MonoBehaviour
    {
        public static Action OnExplodeGates;
        public static Action OnLevelIntroDone;

        [SerializeField] private GameObject _setupCamObj;
        [SerializeField] private GameObject _tpsCamObj;
        [SerializeField] private GameObject _levelIntroCamObj;
        [SerializeField] private GameObject _speakHudObj;

        [Space(8)]
        [SerializeField] private GameObject _mainCamObj;

        private CinemachineVirtualCamera _levelIntroCam;
        private CinemachineVirtualCamera _tpsCam;
        private CinemachineTrackedDolly _levelIntroDollyPath;

        #region Unity Messages
        private void OnEnable()
        {
            GameLoopController.OnChangeGameState += OnChangeGameState;
            GameStartAction.OnSelectCannon += OnSelectTurret;
        }

        private void OnDisable()
        {
            GameLoopController.OnChangeGameState -= OnChangeGameState;
            GameStartAction.OnSelectCannon -= OnSelectTurret;
        }

        private void Start()
        {
            _levelIntroCam = _levelIntroCamObj.GetComponent<CinemachineVirtualCamera>();
            _tpsCam = _tpsCamObj.GetComponent<CinemachineVirtualCamera>();
            _levelIntroDollyPath = _levelIntroCam.GetCinemachineComponent<CinemachineTrackedDolly>();
            ActivateSetupCam();
        }
        #endregion

        private void ActivateTPSCam()
        {
            // TODO: Statemachine
            _tpsCamObj.SetActive(true);
            _setupCamObj.SetActive(false);
            _levelIntroCamObj.SetActive(false);
            _speakHudObj.SetActive(false);
            _mainCamObj.tag = "MainCamera";
        }

        private void ActivateSetupCam()
        {
            _setupCamObj.SetActive(true);
            _tpsCamObj.SetActive(false);
            _levelIntroCamObj.SetActive(false);
            _speakHudObj.SetActive(true);
            _mainCamObj.tag = "Untagged";
        }

        private void ActivateLevelIntroCam()
        {
            _levelIntroCamObj.SetActive(true);
            _setupCamObj.SetActive(false);
            _tpsCamObj.SetActive(false);
            _speakHudObj.SetActive(false);
            _mainCamObj.tag = "Untagged";

            _levelIntroDollyPath.m_PathPosition = 0.0f;
            StartCoroutine(LevelIntroTrackRoutine());
            //_levelIntroCam.setpath
        }

        private IEnumerator LevelIntroTrackRoutine()
        {
            while (_levelIntroDollyPath.m_PathPosition < 1.0f)
            {
                _levelIntroDollyPath.m_PathPosition += Time.deltaTime * 0.5f;
                yield return null;
            }

            yield return new WaitForSeconds(0.2f);
            OnExplodeGates?.Invoke();

            yield return new WaitForSeconds(1.0f);
            OnLevelIntroDone?.Invoke();
        }

        #region System.Action Handlers
        private void OnChangeGameState(GameState currentGameState)
        {
            switch (currentGameState)
            {
                case GameState.LevelIntro:
                    ActivateLevelIntroCam();
                    break;

                case GameState.ShootMode:
                    ActivateTPSCam();
                    break;

                default:
                    break;
            }
        }

        private void OnSelectTurret(TurretSpawner turretSpawner)
        {
            turretSpawner.SetTpsCam(_tpsCam);
        }
        #endregion
    }
}
