using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.LEGO.Game;

namespace Lego.SummerJam.NoFrogsAllowed
{
    public class Frog : MonoBehaviour
    {
        public static Action<Frog> OnSpawn;
        public static Action<Frog> OnDespawn;

        #region Serialized Fields
        [SerializeField] private GameObject _frogModel;
        [SerializeField] private GameObject _coinDropParticle;

        [Space(8)]
        [SerializeField] private float _life = 5.0f;
        [SerializeField] private float _jumpStrength = 2.0f;
        [SerializeField] private float _jumpPause = 0.0f;
        [SerializeField] private int _damage = 1;
        [SerializeField] private int _coinDrop = 10;
        
        [Space(10)]
        // Note: The following 'Variable(s)' was created using LEGO Microgame Editors
        // It's a ScriptableObject located at Assets/LEGO/Scriptable Objects
        [SerializeField] private Variable _baseHealth;
        [SerializeField] private Variable _coins;
        [SerializeField] private Variable _enemyOnField;
        #endregion

        private Transform _target;
        private Rigidbody _rb;
        private float _slowRate;
        private bool _isDead = false;
        private float _pauseTime = 0.0f;

        private void Start()
        {
            _rb = GetComponent<Rigidbody>();
            _rb.velocity = Vector3.zero;
            _slowRate = 0.0f;
            OnSpawn?.Invoke(this);
        }

        private void OnDestroy()
        {
            OnDespawn?.Invoke(this);
        }

        private void FixedUpdate()
        {
            _pauseTime += Time.deltaTime;
            if (_target == null || !IsGrounded() || _pauseTime <= _jumpPause)
            {
                return;
            }

            _pauseTime = UnityEngine.Random.Range(0.0f, 1.0f);
            Vector3 jumpVector = _target.position - transform.position;
            jumpVector.y = 0;
            jumpVector.y = jumpVector.magnitude * 2.0f;// * 5;
            jumpVector.Normalize();

            float randMult = UnityEngine.Random.Range(0.8f, 1.0f);
            jumpVector *= _jumpStrength * (randMult - _slowRate);

            _rb.velocity = Vector3.zero;
            _rb.AddForce(jumpVector, ForceMode.Impulse);
            //_rb.velocity = Vector3.Min(_rb.velocity, _rb.velocity.normalized * 2.0f);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Base"))
            {
                int baseHealth = VariableManager.GetValue(_baseHealth);
                VariableManager.SetValue(_baseHealth, baseHealth - _damage);

                int enemyOnField = VariableManager.GetValue(_enemyOnField);
                VariableManager.SetValue(_enemyOnField, enemyOnField - 1);

                Destroy(gameObject);
            }
        }

        public void Damage(float damage)
        {
            if (_isDead)
            {
                return;
            }

            _life -= damage;
            if (_life <= 0)
            {
                int coinCount = VariableManager.GetValue(_coins);
                VariableManager.SetValue(_coins, coinCount + _coinDrop);

                int enemyOnField = VariableManager.GetValue(_enemyOnField);
                VariableManager.SetValue(_enemyOnField, enemyOnField - 1);

                _isDead = true;
                StartCoroutine(CoinDropRoutine());
            }
        }

        public void Slow(float slowRate)
        {
            _slowRate = slowRate;
        }

        public void SetTarget(Transform target)
        {
            _target = target;
        }

        // TODO: Collide with 'Environment' layers only
        // source: https://gamedev.stackexchange.com/questions/105399/how-to-check-if-grounded-with-rigidbody
        private bool IsGrounded()
        {
            LayerMask mask = LayerMask.NameToLayer("Environment");
            RaycastHit[] hits;

            //We raycast down 1 pixel from this position to check for a collider
            float radius = 0.2f;
            Vector3 positionToCheck = transform.position;
            positionToCheck.y -= radius * 1.01f;
            hits = Physics.SphereCastAll(positionToCheck, radius, new Vector3(0, -1, 0), radius, 1 << mask.value);

            //if a collider was hit, we are grounded
            bool isGrounded = hits.Length > 0;
            //Debug.Log("grounded? " + isGrounded.ToString());
            return isGrounded;
        }

        private IEnumerator CoinDropRoutine()
        {
            _frogModel.SetActive(false);
            _coinDropParticle.SetActive(true);
            yield return new WaitForSeconds(2.0f);
            Destroy(gameObject);
        }
    }
}
