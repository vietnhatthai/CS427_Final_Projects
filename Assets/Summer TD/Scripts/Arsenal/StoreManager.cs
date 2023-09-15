using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lego.SummerJam.NoFrogsAllowed
{
    public class StoreManager : MonoBehaviour
    {
        [SerializeField] private List<TarSpawner> _tarSpawnerList;
        [SerializeField] private List<SpikeTrapSpawner> _spikeSpawnerList;

        private GameProgressData _gameProgress;

        private void Awake()
        {
            _gameProgress = AssetResources.GameProgress;
        }

        private void Start()
        {
            if (_gameProgress.Data.Level == 0)
            {
                for (int idx = 0; idx < _tarSpawnerList.Count; idx++)
                {
                    _spikeSpawnerList[idx].HideAll();
                    _tarSpawnerList[idx].HideAll();
                }

                return;
            }    

            for (int idx = 0; idx < _tarSpawnerList.Count; idx++)
            {
                _spikeSpawnerList[idx].ShowSpikeSeller();
                _tarSpawnerList[idx].ShowSeller();
            }

            foreach (TrapDataModel trap in _gameProgress.Data.TrapList)
            {
                switch (trap.Type)
                {
                    case TrapType.Spikes:
                        _spikeSpawnerList[trap.ID].ShowSpikeTrap();
                        break;

                    case TrapType.Tar:
                        _tarSpawnerList[trap.ID].ShowTar();
                        break;

                    default:
                        break;
                }
            }
        }
    }
}
