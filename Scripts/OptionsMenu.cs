using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour {

    public Dropdown qualityDropdown, resolutionDropdown;
    public Toggle fullscreenToggle;
    int[] width, height;

    public Slider music,ambience, effects;

    public AudioControl audioControl;

    public void SetUp()
    {
        width = new int[5];
        width[0] = 1920;
        width[1] = 1600;
        width[2] = 1280;
        width[3] = 960;
        width[4] = 800;

        height = new int[5];
        height[0] = 1080;
        height[1] = 900;
        height[2] = 720;
        height[3] = 540;
        height[4] = 450;


        OptionsData data = SaveLoadSystem.LoadOptions();

        qualityDropdown.value = data.quality;
        resolutionDropdown.value = data.resolution;
        fullscreenToggle.isOn = data.fullscreen;
        music.value = data.music;
        ambience.value = data.ambience;
        effects.value = data.effects;


        //SetGraphicsSettings();
        SetAudioSettings();
    }

    public void SetGraphicsSettings()
    {

        Screen.SetResolution(width[resolutionDropdown.value], height[resolutionDropdown.value], fullscreenToggle.isOn);
        QualitySettings.SetQualityLevel(qualityDropdown.value);
        

        SaveLoadSystem.SaveOptions(new OptionsData(resolutionDropdown.value, qualityDropdown.value, fullscreenToggle.isOn,music.value ,ambience.value, effects.value));
    }
    public void SetAudioSettings()
    {
        audioControl.mixer[0].SetFloat("Volume", music.value);
        audioControl.mixer[1].SetFloat("Volume", ambience.value);
        audioControl.mixer[2].SetFloat("Volume", effects.value);


        SaveLoadSystem.SaveOptions(new OptionsData(resolutionDropdown.value, qualityDropdown.value, fullscreenToggle.isOn, music.value, ambience.value, effects.value));
    }
}
