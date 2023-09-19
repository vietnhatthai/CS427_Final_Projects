using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace DialogSystem
{
    public class DialogBaseClass : MonoBehaviour
    {
        [SerializeField] GameObject dialogGameObject;
        
        public MultipleLine conversationInJson = new MultipleLine();

        public bool finished = false;
        public bool isProcessing = false;
        protected IEnumerator WriteText(TextAsset jsonFile, Text textHolder, Color textColor, Font textFont, float delay)
        {
            isProcessing = true;
            conversationInJson = JsonUtility.FromJson<MultipleLine>(jsonFile.text);
            textHolder.color = textColor;
            textHolder.font = textFont;
            foreach (Line line in conversationInJson.conversation)
            {
                textHolder.text = string.Empty;
                for (int i = 0; i < line.text.Length; ++i)
                {
                    textHolder.text += line.text[i];
                    if (finished == true)
                    {
                        textHolder.text = line.text;
                        // finished = false;
                        yield return new WaitForSeconds((float)0.5);
                        break;
                    }
                    yield return new WaitForSeconds(delay);
                }
                yield return new WaitUntil(() => Input.GetMouseButton(0));
                finished = false;
            }
            isProcessing = false;
            dialogGameObject.SetActive(false);
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                finished = true;
            }
        }
    }
}
