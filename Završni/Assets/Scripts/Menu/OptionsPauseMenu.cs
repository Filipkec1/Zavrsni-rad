using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;
using UnityEngine.UI;

public class OptionsPauseMenu : MonoBehaviour
{
    //resolution
    Resolution[] resolutions;
    public TMPro.TMP_Dropdown resolutionDropdown;

    //fullscreen
    public Toggle fullscreenTogle;
    bool isFullscreen = false;
    int screenInt;

    //graphics
    public TMPro.TMP_Dropdown graphicsDropdown;

    //audio
    public AudioMixer audioMixer;
    public Slider volumeSlider;

    //player input
    public TMPro.TMP_InputField sensitivityInput;

    //const
    const string prefName = "optionValue";
    const string resName = "resolutionOption";

    private void Awake()
    {
        screenInt = PlayerPrefs.GetInt("togglestate");

        if (screenInt == 1)
        {
            isFullscreen = true;
            fullscreenTogle.isOn = true;
        }
        else
        {
            isFullscreen = false;
            fullscreenTogle.isOn = false;
        }

        resolutionDropdown.onValueChanged.AddListener(new UnityAction<int>(index =>
        {
            PlayerPrefs.SetInt(resName, resolutionDropdown.value);
            PlayerPrefs.Save();

        }));

        graphicsDropdown.onValueChanged.AddListener(new UnityAction<int>(index =>
        {
            PlayerPrefs.SetInt(prefName, graphicsDropdown.value);
            PlayerPrefs.Save();

        }));
    }


    private void Start()
    {
        sensitivityInput.characterValidation = TMPro.TMP_InputField.CharacterValidation.Integer;
        sensitivityInput.text = PlayerPrefs.GetInt("sensitivity", 100).ToString();

        volumeSlider.value = PlayerPrefs.GetFloat("mVolume", 1f);
        audioMixer.SetFloat("VolumeMixer", PlayerPrefs.GetFloat("mVolume"));

        graphicsDropdown.value = PlayerPrefs.GetInt(prefName, 3);

        resolutions = Screen.resolutions;
        List<string> resolutionOptions = new List<string>();

        resolutionDropdown.ClearOptions();
        int currentResolutionIndex = 0;

        for (int i = 0; i < resolutions.Length; i++)
        {
            resolutionOptions.Add(resolutions[i].width + " x " + resolutions[i].height + " " + resolutions[i].refreshRate + "Hz");

            if (resolutions[i].width == Screen.currentResolution.width &&
                resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }

        resolutionDropdown.AddOptions(resolutionOptions);
        resolutionDropdown.value = PlayerPrefs.GetInt(resName, currentResolutionIndex);
        resolutionDropdown.RefreshShownValue();
    }

    public void SetResolution(int resolutionIndex)
    {
        Screen.SetResolution(resolutions[resolutionIndex].width, resolutions[resolutionIndex].height, Screen.fullScreen);
    }

    public void SetFullscreen(bool _isFullScreen)
    {
        isFullscreen = _isFullScreen;
        Screen.fullScreen = _isFullScreen;

        if (!(isFullscreen))
        {
            PlayerPrefs.SetInt("togglestate", 0);
        }
        else
        {
            PlayerPrefs.SetInt("togglestate", 1);
        }
    }

    public void SetQuality(int qualityIndex)
    {
        PlayerPrefs.SetInt(prefName, qualityIndex);
        QualitySettings.SetQualityLevel(PlayerPrefs.GetInt(prefName, qualityIndex));
    }

    public void SetVolume(float volume)
    {
        PlayerPrefs.SetFloat("mVolume", volume);
        audioMixer.SetFloat("VolumeMixer", PlayerPrefs.GetFloat("mVolume"));
    }

    public void SetSensitivity(string _sensitivity)
    {
        int sensitivityInt = Int32.Parse(_sensitivity);

        PlayerPrefs.SetInt("sensitivity", sensitivityInt);
        PlayerPrefs.Save();

        PlayerCamera.instance.sensitivity = sensitivityInt;
    }
}
