using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

namespace Lego.SummerJam.NoFrogsAllowed
{
    [CreateAssetMenu(fileName = "NewGameProgressData", menuName = "ScriptableObjects/NoFrogsAllowed/GameProgressData", order = 1)]
    public class GameProgressData : ScriptableObject
    {
        private const string PK_NFA_GAMEDATA = "Lego.SummerJam.NoFrogsAllowed.GameProgressData";
        private const int DEFAULT_MONEY = 600;
        private const int DEFAULT_LEVEL = 0;
        private const int LAST_LEVEL = 3;

        public GameProgressDataModel Data { get; set; }

        public bool IsLastLevel
        {
            get => Data.Level >= LAST_LEVEL;
        }

        public void LoadDefaults()
        {
            Data = new GameProgressDataModel
            {
                Level = DEFAULT_LEVEL,
                Money = DEFAULT_MONEY,
                WeaponList = new HashSet<WeaponDataModel>(),
                TrapList = new HashSet<TrapDataModel>()
            };
        }

        public void LoadData()
        {
            //PlayerPrefs.DeleteKey(PK_NFA_GAMEDATA);
            string json = PlayerPrefs.GetString(PK_NFA_GAMEDATA, string.Empty);
            if (string.IsNullOrEmpty(json))
            {
                LoadDefaults();
                return;
            }

            Data = JsonConvert.DeserializeObject<GameProgressDataModel>(json);
        }

        public void SaveData()
        {
            string json = JsonConvert.SerializeObject(Data);
            PlayerPrefs.SetString(PK_NFA_GAMEDATA, json);
            //PlayerPrefs.Save();
        }
    }
}
