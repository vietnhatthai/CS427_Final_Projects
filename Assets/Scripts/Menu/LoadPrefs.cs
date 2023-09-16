using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Audio;

public class LoadPrefs : MonoBehaviour
{

    [Header("General Setting")]
    [SerializeField] private bool canUse = false;
    [SerializeField] private MenuController menuController;

    [Header("Volume Setting")]
    [SerializeField] private TextMeshProUGUI volumeTextValue = null;
    [SerializeField] private Slider volumeSlider = null;

    [Header("Brightness Setting")]
    [SerializeField] private Slider brightnessSlider = null;
    [SerializeField] private TextMeshProUGUI brightnessTextValue = null;

    [Header("Quality Setting")]
    [SerializeField] private TMP_Dropdown qualityDropdown = null;

    [Header("Fullscreen Setting")]
    [SerializeField] private Toggle fullscreenToggle = null;

    [Header("Sensitivity Setting")]
    [SerializeField] private TextMeshProUGUI controllerSenTextValue = null;
    [SerializeField] private Slider controllerSenSlider = null;

    [Header("Invert Y Setting")]
    [SerializeField] private Toggle invertYToggle = null;

    private void Awake()
    {
        if (canUse)
        {
            // LoadSettings();
            if (PlayerPrefs.HasKey("masterVolume"))
            {
                float volume = PlayerPrefs.GetFloat("masterVolume");

                volumeSlider.value = volume;
                volumeTextValue.text = volume.ToString("0.0");
                AudioListener.volume = volume;
            }
            else
            {
                menuController.ResetButtion("Audio");
            }

            if (PlayerPrefs.HasKey("masterQuality"))
            {
                int quality = PlayerPrefs.GetInt("masterQuality");

                qualityDropdown.value = quality;
                QualitySettings.SetQualityLevel(quality);
            }

            if (PlayerPrefs.HasKey("masterFullscreen"))
            {
                int fullscreen = PlayerPrefs.GetInt("masterFullscreen");

                if (fullscreen == 1)
                {
                    fullscreenToggle.isOn = true;
                    Screen.fullScreen = true;
                }
                else
                {
                    fullscreenToggle.isOn = false;
                    Screen.fullScreen = false;
                }
            }

            // brightness
            if (PlayerPrefs.HasKey("masterBrightness"))
            {
                float brightness = PlayerPrefs.GetFloat("masterBrightness");

                brightnessSlider.value = brightness;
                brightnessTextValue.text = brightness.ToString("0.0");
                // change the brightness
            }

            // sensitivity
            if (PlayerPrefs.HasKey("masterSen"))
            {
                float sensitivity = PlayerPrefs.GetFloat("masterSen");

                controllerSenSlider.value = sensitivity;
                controllerSenTextValue.text = sensitivity.ToString("0");
                menuController.mainControllerSen = Mathf.RoundToInt(sensitivity);
            }

            // invert Y
            if (PlayerPrefs.HasKey("masterInvertY"))
            {
                int invertY = PlayerPrefs.GetInt("masterInvertY");

                if (invertY == 1)
                {
                    invertYToggle.isOn = true;
                }
                else
                {
                    invertYToggle.isOn = false;
                }
            }
        }
    }

}

