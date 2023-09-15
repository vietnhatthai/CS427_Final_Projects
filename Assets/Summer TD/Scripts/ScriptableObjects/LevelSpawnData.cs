using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lego.SummerJam.NoFrogsAllowed
{

    [CreateAssetMenu(fileName = "NewLevelSpawnData", menuName = "ScriptableObjects/NoFrogsAllowed/LevelSpawnData", order = 1)]
    public class LevelSpawnData : ScriptableObject
    {
        [SerializeField] private List<WaveSpawnData> _waveSpawnDataList;

        public List<WaveSpawnData> SpawnList => _waveSpawnDataList;
    }
}
