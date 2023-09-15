using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lego.SummerJam.NoFrogsAllowed
{
    public class TarController : MonoBehaviour
    {
        [SerializeField] private float _slowRate;

        // Start is called before the first frame update
        private void OnTriggerEnter(Collider collider)
        {
            //Debug.Log("triggered by " + collider.name);
            Frog frog = collider.GetComponent<Frog>();
            if (frog != null)
            {
                Debug.Log("SLOW!");
                frog.Slow(_slowRate);
            }
        }

        private void OnTriggerExit(Collider collider)
        {
            //Debug.Log("triggered by " + collider.name);
            Frog frog = collider.GetComponent<Frog>();
            if (frog != null)
            {
                frog.Slow(0.0f);
            }
        }
    }
}
