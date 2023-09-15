using System;
using System.Collections.Generic;
using UnityEngine;

namespace Lego.SummerJam.NoFrogsAllowed
{
    [Serializable]
    public class WaveSpawnData
    {
        [SerializeField] private float _waveDelay;
        [SerializeField] private float _spawnInterval;
        [SerializeField] private int _spawnCount;
        [SerializeField] private GameObject _enemy;

        public float Delay => _waveDelay;
        public float Interval => _spawnInterval;
        public int MaxSpawnCount => _spawnCount;
        public GameObject EnemyPrefab => _enemy;
    }
}
