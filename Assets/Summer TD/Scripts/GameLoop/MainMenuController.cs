using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lego.SummerJam.NoFrogsAllowed
{
    public class MainMenuController : MonoBehaviour
    {
        private GameProgressData _gameProgress;

        private void Awake()
        {
            _gameProgress = AssetResources.GameProgress;
        }

        public void PlayNewGame()
        {
            _gameProgress.LoadDefaults();
            _gameProgress.SaveData();
        }
    }
}
