using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lego.SummerJam.NoFrogsAllowed
{
    public enum WeaponType
    { 
        Cannon
    }

    public class WeaponDataModel
    {
        public int ID { get; set; }
        public WeaponType Type { get; set; }

        public override bool Equals(object obj)
        {
            return obj is WeaponDataModel model &&
                   ID == model.ID;
        }

        public override int GetHashCode()
        {
            return 1213502048 + ID.GetHashCode();
        }
    }
}
