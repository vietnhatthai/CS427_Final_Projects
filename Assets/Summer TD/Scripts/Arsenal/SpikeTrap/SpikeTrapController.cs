using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lego.SummerJam.NoFrogsAllowed
{
    public class SpikeTrapController : MonoBehaviour
    {
        [SerializeField] private float _damage;
        [SerializeField] private Transform _spikesTransform;

        private Coroutine _attackRoutine;
        private bool _isOut;

        #region Unity Messages
        private void OnEnable()
        {
            GameLoopController.OnChangeGameState += OnChangeGameState;
        }

        private void OnDisable()
        {
            GameLoopController.OnChangeGameState -= OnChangeGameState;
        }

        private void OnTriggerEnter(Collider collider)
        {
            //Debug.Log("triggered by " + collider.name);
            Frog frog = collider.GetComponent<Frog>();
            if (frog != null && _isOut)
            {
                //Debug.Log("FROG DAMAGE!");
                frog.Damage(_damage);
            }
        }
        #endregion

        private IEnumerator AttackRoutine()
        {
            float hidePosY = -0.8f;
            float outPosY = 0.2f;
            Vector3 pos = _spikesTransform.localPosition;
            pos.y = hidePosY;
            _spikesTransform.localPosition = pos;

            while (true)
            {
                while (pos.y < outPosY)
                {
                    pos.y += 0.1f;
                    _spikesTransform.localPosition = pos;
                    yield return null;
                }

                //Debug.Log("Spike OUT!");
                pos.y = outPosY;
                _spikesTransform.localPosition = pos;
                _isOut = true;
                yield return new WaitForSeconds(2.0f);

                while (pos.y > hidePosY)
                {
                    pos.y -= Time.deltaTime * 0.5f;
                    _spikesTransform.localPosition = pos;
                    yield return null;
                }

                //Debug.Log("Spike HIDDEN!");
                pos.y = hidePosY;
                _spikesTransform.localPosition = pos;
                _isOut = false;
                yield return new WaitForSeconds(2.0f);
            }
        }

        #region System.Action Handlers
        private void OnChangeGameState(GameState currentGameState)
        {
            switch (currentGameState)
            {
                case GameState.ShootMode:
                    {
                        _attackRoutine = StartCoroutine(AttackRoutine());
                        break;
                    }

                default:
                    {
                        _isOut = false;
                        if (_attackRoutine != null)
                        {
                            StopCoroutine(_attackRoutine);
                        }

                        break;
                    }
            }
        }
        #endregion
    }
}
