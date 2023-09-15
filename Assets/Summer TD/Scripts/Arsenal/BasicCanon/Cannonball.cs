using System.Linq;
using System.Collections;
using UnityEngine;

namespace Lego.SummerJam.NoFrogsAllowed
{
    public class Cannonball : MonoBehaviour
    {
        [SerializeField] private GameObject _shell;
        [SerializeField] private GameObject _explodeParticle;

        [Space(8)]
        [SerializeField] private float _explosionStrength = 2.0f;
        [SerializeField] private float _explosionRadius = 5.0f;
        [SerializeField] private float _damage = 2.0f;

        private bool _exploded = false;

        private void OnCollisionEnter(Collision collision)
        {
            if (_exploded)
            {
                return;
            }

            int hitLayer = collision.gameObject.layer;
            string hitLayerName = LayerMask.LayerToName(hitLayer);
            string[] maskNames = new string[] { "Enemy", "Environment" };
            LayerMask mask = LayerMask.GetMask(maskNames);
            if (!maskNames.ToList().Contains(hitLayerName))
            {
                return;
            }

            //Debug.Log("on hit: " + hitLayerName);
            Collider[] colliderHits = Physics.OverlapSphere(transform.position, _explosionRadius, mask);
            foreach (Collider hit in colliderHits)
            {
                Frog frog = hit.gameObject.GetComponent<Frog>();
                if (frog == null)
                {
                    continue;
                }

                frog.Damage(_damage);
                Rigidbody rb = hit.GetComponent<Rigidbody>();

                if (rb != null)
                {
                    //Debug.Log("explosion affect " + frog.gameObject.name);
                    //rb.AddExplosionForce(_explosionStrength, transform.position, _explosionRadius);
                    Vector3 forceDir = (frog.transform.position - transform.position).normalized * _explosionStrength;
                    rb.velocity = Vector3.zero;
                    rb.AddForceAtPosition(forceDir, transform.position, ForceMode.Impulse);
                }
            }

            StartCoroutine(ExplodeRoutine());
        }

        private IEnumerator ExplodeRoutine()
        {
            _exploded = true;
            _shell.SetActive(false);
            _explodeParticle.SetActive(true);

            yield return new WaitForSeconds(2.0f);
            Destroy(gameObject);
        }
    }
}
