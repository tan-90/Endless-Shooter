using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour {

    public GameObject MainMenuHolder;
    public GameObject OptionsMenuHolder;

    public Slider[] VolumeSliders;
    public Toggle[] ResolutionToggles;
    public int[] ScreenWidths;
    int CurrentResolutionIndex;

    public void Play()
    {
        SceneManager.LoadScene("Game");
    }
    public void Quit()
    {
        Application.Quit();
    }
    public void OptionsMenu()
    {
        MainMenuHolder.SetActive(false);
        OptionsMenuHolder.SetActive(true);
    }
    public void MainMenu()
    {
        MainMenuHolder.SetActive(true);
        OptionsMenuHolder.SetActive(false);
    }

    public void SetScreenResolution(int i)
    {
        if(ResolutionToggles[i].isOn)
        {
            CurrentResolutionIndex = i;
            float AspectRatio = 16 / 9f;
            Screen.SetResolution(ScreenWidths[i], (int)(ScreenWidths[i] / AspectRatio), false);
            PlayerPrefs.SetInt("ScreenResIndex", CurrentResolutionIndex);
            PlayerPrefs.Save();
        }
    }
    public void SetFullScreen(bool FullScreen)
    {
        for(int i = 0; i < ResolutionToggles.Length; i++)
        {
            ResolutionToggles[i].interactable = !FullScreen;
        }
        if(FullScreen)
        {
            Resolution[] MonitorResolutions = Screen.resolutions;
            Resolution MaxResolution = MonitorResolutions[MonitorResolutions.Length - 1];
            Screen.SetResolution(MaxResolution.width, MaxResolution.height, true);
            PlayerPrefs.SetInt("Fullscreen", 1);
            PlayerPrefs.Save();
        }
        else
        {
            SetScreenResolution(CurrentResolutionIndex);
            PlayerPrefs.SetInt("Fullscreen", 0);
            PlayerPrefs.Save();
        }
    }

    public void SetMasterVolume(float Value)
    {
        AudioManager.INSTANCE.SetVolume(Value, AudioManager.AudioChannel.Master);
    }
    public void SetMusicVolume(float Value)
    {
        AudioManager.INSTANCE.SetVolume(Value, AudioManager.AudioChannel.Music);
    }
    public void SetSfxVolume(float Value)
    {
        AudioManager.INSTANCE.SetVolume(Value, AudioManager.AudioChannel.Sfx);
    }

    // Use this for initialization
    void Start()
    {
        CurrentResolutionIndex = PlayerPrefs.GetInt("ScreenResIndex");
        bool IsFullScreen = PlayerPrefs.GetInt("Fullscreen") == 1;
        VolumeSliders[0].value = AudioManager.INSTANCE.MasterVolumePercent;
        VolumeSliders[1].value = AudioManager.INSTANCE.MusicVolumePercent;
        VolumeSliders[2].value = AudioManager.INSTANCE.SfxVolumePercent;

        for (int i = 0; i < ResolutionToggles.Length; i++)
        {
            ResolutionToggles[i].isOn = i == CurrentResolutionIndex;
        }

        SetFullScreen(IsFullScreen);
        SetMasterVolume(VolumeSliders[0].value);
        SetMusicVolume(VolumeSliders[1].value);
        SetSfxVolume(VolumeSliders[2].value);
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
