using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
{
    public AudioMixer mixer;
    public Slider musicSlider;
    public Slider soundSlider;
    public Toggle fullscreenToggle;
    public Toggle musicToggle;
    public Toggle soundToggle;
    public Dropdown resolutionDropdown;
    public Dropdown qualityDropdown;
    public Toggle showTutorialToggle;
    public Button resetButton;
    public MainPanel mainPanel;

    public Animator optionsAnimator;
    public Animator mainAnimator;

    Resolution[] resolutions;

    private Animator anim;

    private bool firstTimeAwake = true;

    private void Start()
    {

        if (firstTimeAwake)
            Screen.SetResolution(1280, 720, false);

        firstTimeAwake = false;

        // ---- Fullscreen Toggle
        fullscreenToggle.isOn = Screen.fullScreenMode == FullScreenMode.FullScreenWindow;

        // ---- Resolutions
        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();

        List<string> resolutionNames = new List<string>();
        int currentIndex = 0;
        resolutionNames.Add("Choose");
        for(int i = 0; i < resolutions.Length; i++)
        {
            resolutionNames.Add(resolutions[i].width + "x" + resolutions[i].height + " @" + resolutions[i].refreshRate);
        }
        resolutionDropdown.AddOptions(resolutionNames);
        resolutionDropdown.SetValueWithoutNotify(currentIndex);

        // ---- Quality
        string[] qNames = QualitySettings.names;
        qualityDropdown.ClearOptions();
        qualityDropdown.AddOptions(new List<string>(qNames));
        qualityDropdown.value = QualitySettings.GetQualityLevel();



        // ---- Audio
        //SetMusicLevel(GameManager.Instance.MusicLevel);
        musicSlider.value = GameManager.Instance.MusicLevel;
        soundSlider.value = GameManager.Instance.SoundLevel;
        musicToggle.isOn = !GameManager.Instance.MusicMuted;
        soundToggle.isOn = !GameManager.Instance.SoundMuted;

        // ---- Gameplay
        UpdateResetButton();
        showTutorialToggle.isOn = GameManager.Instance.ShowTutorial;

    }

    public void ResolutionDropdownChanged(int index)
    {
        index--; // Because of the "Choose" option, a resolution name of 1 has a resolution index of 0
        if (index < 0) return;
        //Screen.SetResolution()
        //Screen.fullScreenMode = FullScreenMode.Windowed;
        Screen.SetResolution(resolutions[index].width, resolutions[index].height, fullscreenToggle.isOn, resolutions[index].refreshRate);
        //SetFullscreen(fullscreenToggle.isOn);
    }

    public void SetFullscreen(bool value)
    {
        Screen.fullScreenMode = value ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed;
    }

    public void SetQuality(int index)
    {
        QualitySettings.SetQualityLevel(index);
    }

    public void SetMusicLevel(float level)
    {
        mixer.SetFloat("MusicLevel", level);
        GameManager.Instance.MusicLevel = level;
        musicToggle.isOn = true;
    }

    public void SetSoundLevel(float level)
    {
        mixer.SetFloat("SoundLevel", level);
        GameManager.Instance.SoundLevel = level;
        soundToggle.isOn = true;
    }

    public void ToggleMusic(bool value)
    {
        if(!value)
        {
            mixer.SetFloat("MusicLevel", -80f);
        }
        else
        {
            mixer.SetFloat("MusicLevel", GameManager.Instance.MusicLevel);
        }

        GameManager.Instance.MusicMuted = value;

    }

    public void ToggleSound(bool value)
    {
        if (!value)
        {
            mixer.SetFloat("SoundLevel", -80f);
        }
        else
        {
            mixer.SetFloat("SoundLevel", GameManager.Instance.SoundLevel);
        }


        GameManager.Instance.SoundMuted = value;
    }

    public void ToggleShowTutorial(bool value)
    {
        GameManager.Instance.ShowTutorial = value;
    }

    public void ResetPlayerData()
    {
        GameManager.Instance.ResetPlayerData();
        UpdateResetButton();
        mainPanel.Start();
        GameManager.Instance.SavePlayerData();
    }

    public void BackButtonClicked()
    {
        GameManager.Instance.SavePlayerData();
        optionsAnimator.SetTrigger("Exit");
        mainAnimator.SetTrigger("Enter");
    }

    void UpdateResetButton()
    {
        if (GameManager.Instance.PlayerDataExists)
            resetButton.interactable = true;
        else
            resetButton.interactable = false;
    }

    
}
