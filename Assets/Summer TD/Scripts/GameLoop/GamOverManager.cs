using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Lego.SummerJam.NoFrogsAllowed
{
    public class GamOverManager : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _gameOverMessage;
        [SerializeField] private TextMeshProUGUI _playButtonText;

        private GameProgressData _gameProgress;
        // execution script

        private void Start()
        {
            _gameProgress = AssetResources.GameProgress;
            _gameProgress.LoadData();
            if (_gameProgress.Data.Win)
            {
                Win();
            }
            else
            {
                Lose();
            }
        }

        private void OnDisable()
        {
            _gameProgress.SaveData();
        }

        private void Win()
        {
            if (_gameProgress.IsLastLevel)
            {
                _gameProgress.LoadDefaults();
                _gameOverMessage.text = "Thank you for playing!";
                _playButtonText.text = "Play Again";
                return;
            }

            string[] message = new string[] 
            {
                "Great Defense!",
                "You're Amazing!",
                "Awesome!",
                "Please teach me, master."
            };

            _gameProgress.Data.Level += 1;
            _gameOverMessage.text = message[Random.Range(0, message.Length)];
            _playButtonText.text = "Next Level";
        }

        private void Lose()
        {
            string[] message = new string[]
            {
                "You can do better next time.",
                "Frogs are all over the place.",
                "Try a new strategy."
            };

            _gameOverMessage.text = message[Random.Range(0, message.Length)];
            _playButtonText.text = "Try Again!";
        }
    }
}
