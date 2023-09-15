using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

namespace Lego.SummerJam.NoFrogsAllowed
{
    public class DialogueManager : MonoBehaviour
    {
        [Serializable]
        private class CamShot
        {
            [SerializeField] private GameObject _camObj;
            [SerializeField] private GameObject _speakObj;
            [SerializeField] private float _duration;

            public GameObject CamObj => _camObj;
            public GameObject SpeakObj => _speakObj;
            public float Duration => _duration;
        }

        [SerializeField] private List<CamShot> _camShotList; 

        private void Awake()
        {
            ResetAllCam();
            StartCoroutine(StartMovie());
        }

        private void ResetAllCam()
        {
            foreach (CamShot camShot in _camShotList)
            {
                camShot.CamObj.SetActive(false);
                if (camShot.SpeakObj != null)
                {
                    camShot.SpeakObj.SetActive(false);
                }
            }
        }

        private IEnumerator StartMovie()
        {
            foreach (CamShot camShot in _camShotList)
            {
                camShot.CamObj.SetActive(true);
                if (camShot.SpeakObj != null)
                {
                    yield return new WaitForSeconds(0.2f);
                    camShot.SpeakObj.SetActive(true);
                }

                if (camShot.Duration < 0)
                {
                    continue;
                }

                yield return new WaitForSeconds(camShot.Duration);
                camShot.CamObj.SetActive(false);
                if (camShot.SpeakObj != null)
                {
                    camShot.SpeakObj.SetActive(false);
                }
            }
        }
    }
}
