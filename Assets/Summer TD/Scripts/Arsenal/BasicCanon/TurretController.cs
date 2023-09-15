using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Unity.LEGO.Game;

namespace Lego.SummerJam.NoFrogsAllowed
{
    public class TurretController : MonoBehaviour
    {
        #region Serialized Field
        [SerializeField] private Transform _verticalPivot;
        [SerializeField] private Transform _horizontalPivot;
        [SerializeField] private Transform _playerSeat;
        [SerializeField] private Transform _tpsCamAnchor;

        [Space(8)]
        [SerializeField] private float _horizontalSpeed = 2.0f;
        [SerializeField] private float _verticalSpeed = 2.0f;

        [Space(8)]
        [SerializeField] private GameObject _autoShooter;
        [SerializeField] private GameObject _manualShooter;
        #endregion

        [SerializeField] private Transform _testTarget;

        private Coroutine _aimRoutine;
        private Transform _playerTransform;
        private List<Frog> _frogTargets = new List<Frog>();

        private void OnEnable()
        {
            GameLoopController.OnChangeGameState += OnChangeGameState;
            Frog.OnSpawn += OnSpawnFrog;
            Frog.OnDespawn += OnDespawnFrog;
            EventManager.AddListener<OptionsMenuEvent>(OnGamePause);
        }

        private void OnDisable()
        {
            GameLoopController.OnChangeGameState -= OnChangeGameState;
            Frog.OnSpawn -= OnSpawnFrog;
            Frog.OnDespawn -= OnDespawnFrog;
            EventManager.RemoveListener<OptionsMenuEvent>(OnGamePause);
        }

        //private void Start()
        //{
        //    // DEBUG
        //    StartCoroutine(AutoAimRoutine());
        //}

        private void OnReleaseFrogs()
        {
            if (_playerTransform == null)
            {
                //Debug.Log("Missing player transform / not selected.");
                _autoShooter.SetActive(true);
                _manualShooter.SetActive(false);
                _aimRoutine = StartCoroutine(AutoAimRoutine());
                return;
            }

            _autoShooter.SetActive(false);
            _manualShooter.SetActive(true);
            Vector3 playerSeat = _playerSeat.position;
            playerSeat.y += 0.3f;
            _playerTransform.position = playerSeat;
            _playerTransform.forward = _playerSeat.right;
            _aimRoutine = StartCoroutine(AimRoutine());
        }

        public void OnGamePause(OptionsMenuEvent evt)
        {
            if (evt.Active && _aimRoutine != null)
            {
                StopCoroutine(_aimRoutine);
            }
            else if (!evt.Active && _playerTransform != null)
            {
                _aimRoutine = StartCoroutine(AimRoutine());
            }
            else if (!evt.Active)
            {
                _aimRoutine = StartCoroutine(AutoAimRoutine());
            }
        }

        public void SetPlayer(Transform playerTransform)
        {
            _playerTransform = playerTransform;
        }

        public void SetTpsCam(CinemachineVirtualCamera tpsCam)
        {
            Debug.Log("Set TPS cam.");
            tpsCam.Follow = _tpsCamAnchor;
            tpsCam.LookAt = _tpsCamAnchor;
        }

        #region System.Action Handlers
        private void OnChangeGameState(GameState currentGameState)
        {
            switch (currentGameState)
            {
                case GameState.ShootMode:
                    OnReleaseFrogs();
                    //_gameStartTrigger.SetActive(false);
                    break;

                default:
                    break;
            }
        }

        private void OnSpawnFrog(Frog frog)
        {
            if (_playerTransform != null)
            {
                // selected
                return;
            }

            _frogTargets.Add(frog);
        }

        private void OnDespawnFrog(Frog frog)
        {
            if (_playerTransform != null)
            {
                // selected
                return;
            }

            _frogTargets.Remove(frog);
        }
        #endregion

        private IEnumerator AutoAimRoutine()
        {
            if (_horizontalPivot == null ||
                _verticalPivot == null)
            {
                yield break;
            }

            while (true)
            {
                if (_frogTargets == null || _frogTargets.Count == 0)
                {
                    yield return new WaitForEndOfFrame();
                    continue;
                }

                Transform target = _frogTargets[0].transform;
                //Transform target = _testTarget;
                float step;
                Vector3 targetDirection;
                Quaternion newRotation;

                step = _horizontalSpeed * 0.1f;// Time.deltaTime;
                targetDirection = target.position - _horizontalPivot.position;
                targetDirection.y = _horizontalPivot.position.y;
                newRotation = Quaternion.LookRotation(targetDirection, Vector3.up);
                _horizontalPivot.rotation = Quaternion.RotateTowards(_horizontalPivot.rotation, newRotation, step);

                //step = _verticalSpeed;// * Time.deltaTime;
                //targetDirection = target.position - _verticalPivot.position;
                //newRotation = Quaternion.LookRotation(targetDirection, Vector3.up);
                //Vector3 euler = Quaternion.RotateTowards(_verticalPivot.rotation, newRotation, step).eulerAngles;
                //euler.y = euler.z = 0;
                //_verticalPivot.eulerAngles = euler;

                yield return new WaitForEndOfFrame();
            }
        }

        private IEnumerator AimRoutine()
        {
            if (_horizontalPivot == null ||
                _verticalPivot == null)
            {
                yield break;
            }

            // add delay to wait for camera transition
            yield return new WaitForSecondsRealtime(1.5f);

            while (true)
            {
                float h = _horizontalSpeed * Input.GetAxis("Mouse X");
                _horizontalPivot.Rotate(0, h, 0);

                float v = _verticalSpeed * Input.GetAxis("Mouse Y");
                _verticalPivot.Rotate(v, 0, 0);

                Vector3 playerSeat = _playerSeat.position;
                playerSeat.y += 0.3f;
                _playerTransform.position = playerSeat;
                _playerTransform.forward = _playerSeat.right;

                yield return new WaitForEndOfFrame();
            }
        }
    }
}
