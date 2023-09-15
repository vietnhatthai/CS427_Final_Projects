using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lego.SummerJam.NoFrogsAllowed
{
    public enum TrapType
    { 
        Spikes,
        Tar
    }

    public class TrapDataModel
    { 
        public int ID { get; set; }
        public TrapType Type { get; set; }

        public override bool Equals(object obj)
        {
            return obj is TrapDataModel model &&
                   ID == model.ID;
        }

        public override int GetHashCode()
        {
            return 1213502048 + ID.GetHashCode();
        }
    }
}
