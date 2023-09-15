using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.LEGO.Game;

namespace Lego.SummerJam.NoFrogsAllowed
{
    public class EnemySpawner : MonoBehaviour, IAction
    {
        #region Serialized Fields
        [SerializeField] private Vector2 _spawnArea;
        [SerializeField] private Transform _target;

        [Space(10)]
        // Note: The following 'Variable(s)' was created using LEGO Microgame Editors
        // It's a ScriptableObject located at Assets/LEGO/Scriptable Objects
        [SerializeField] private Variable _enemyToSpawn;
        [SerializeField] private Variable _enemyOnField;
        #endregion

        private LevelSpawnData _levelSpawnData;
        private GameProgressData _gameProgress;

        #region Unity Messages
        private void OnEnable()
        {
            GameLoopController.OnChangeGameState += OnChangeGameState;
        }

        private void OnDisable()
        {
            GameLoopController.OnChangeGameState -= OnChangeGameState;
        }

        private void OnDrawGizmosSelected()
        {
            Vector3 spawnArea = new Vector3(_spawnArea.x, 4, _spawnArea.y);
            Vector3 spawnCenter = transform.position;
            spawnCenter.y = spawnArea.y;

            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(spawnCenter, spawnArea);
        }
        #endregion

        public void Activate()
        {
            _gameProgress = AssetResources.GameProgress;
            _levelSpawnData = Resources.Load<LevelSpawnData>("LevelSpawnData/Level_" + _gameProgress.Data.Level);

            int totalEnemyToSpawn = 0;
            foreach (WaveSpawnData waveData in _levelSpawnData.SpawnList)
            {
                totalEnemyToSpawn += waveData.MaxSpawnCount;
            }

            VariableManager.SetValue(_enemyToSpawn, totalEnemyToSpawn);
            StartCoroutine(StartLevel(_levelSpawnData.SpawnList));
        }

        #region Routines
        private IEnumerator StartLevel(List<WaveSpawnData> waveSpawnList)
        {
            foreach (WaveSpawnData waveData in waveSpawnList)
            {
                yield return new WaitForSeconds(waveData.Delay);
                for (int idx = waveData.MaxSpawnCount; idx > 0; --idx)
                {
                    yield return SpawnEnemy(waveData.Interval, waveData.EnemyPrefab);
                }
            }
        }

        private IEnumerator SpawnEnemy(float waitTime, GameObject enemyPrefab)
        {
            int enemyToSpawn = VariableManager.GetValue(_enemyToSpawn);
            VariableManager.SetValue(_enemyToSpawn, enemyToSpawn - 1);

            int enemyOnField = VariableManager.GetValue(_enemyOnField);
            VariableManager.SetValue(_enemyOnField, enemyOnField + 1);

            GameObject enemyObj = Instantiate(enemyPrefab, transform);
            Vector2 pos = _spawnArea * 0.5f;
            pos.x = Random.Range(-pos.x, pos.x);
            pos.y = Random.Range(-pos.y, pos.y);
            Transform enemyTransform = enemyObj.transform;
            enemyTransform.localPosition = new Vector3(pos.x, 0, pos.y);

            Frog frog = enemyObj.GetComponent<Frog>();
            frog.SetTarget(_target);
            yield return new WaitForSeconds(waitTime);
        }
        #endregion

        #region Game State Handlers

        #endregion

        #region System.Action Handlers
        private void OnChangeGameState(GameState currentGameState)
        {
            switch (currentGameState)
            {
                //case GameState.LevelIntro:
                case GameState.ShootMode:
                    Activate();
                    break;

                default:
                    break;
            }
        }
        #endregion
    }
}
