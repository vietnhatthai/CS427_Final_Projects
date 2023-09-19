using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DialogSystem
{
    public class DialogLine : DialogBaseClass
    {
        [Header ("Text options")]
        private Text textHolder;
        [SerializeField] private Color textColor;
        [SerializeField] private Font textFont;

        [Header("Time parameters")]
        [SerializeField] private float delay;

        // [Header("Sound settings")]
        // [SerializeField] private string Sname;

        public TextAsset jsonFile;

        private void OnEnable()
        {
            Debug.Log("Hello");
            textHolder = GetComponent<Text>();
            StartCoroutine(WriteText(jsonFile, textHolder, textColor, textFont, delay));
        }
    }
}
