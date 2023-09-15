using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lego.SummerJam.NoFrogsAllowed
{
    public static class Constants
    { }

    public static class AssetResources
    { 
        public static readonly GameProgressData GameProgress = Resources.Load<GameProgressData>("GameProgressData");
    }

    public static class SceneId
    { 
        public static readonly string NfaLevelScene = "NFA - Level";
    }
}
