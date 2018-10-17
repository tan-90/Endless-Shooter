using UnityEngine;
using System.Collections;

public class AudioManager : MonoBehaviour {

    public enum AudioChannel
    {
        Master,
        Sfx,
        Music
    };

    public static AudioManager INSTANCE;

    Transform AudioListenerTransform;
    Transform PlayerTransform;


    public float MasterVolumePercent = 1;
    public float SfxVolumePercent = 1;
    public float MusicVolumePercent = 1;

    AudioSource SoundSource2D;
    AudioSource[] MusicSources;
    int MusicSourceIndex;

    SoundLibrary Library;

    void Awake()
    {
        if(INSTANCE != null)
        {
            Destroy(gameObject);
        }else
        {
            INSTANCE = this;
            DontDestroyOnLoad(gameObject);
            Library = GetComponent<SoundLibrary>();
            MusicSources = new AudioSource[2];
            for(int i = 0; i < 2; i++)
            {
                GameObject NewMusicSource = new GameObject("Music source " + (i + 1));
                MusicSources[i] = NewMusicSource.AddComponent<AudioSource>();
                NewMusicSource.transform.parent = transform;
            }

            GameObject NewSoundFX2D = new GameObject("2D sfx source");
            SoundSource2D = NewSoundFX2D.AddComponent<AudioSource>();
            NewSoundFX2D.transform.parent = transform;

            AudioListenerTransform = FindObjectOfType<AudioListener>().transform;
            if(FindObjectOfType<Player>() != null)
            {
                PlayerTransform = FindObjectOfType<Player>().transform;
            }
        }

        MasterVolumePercent = PlayerPrefs.GetFloat("Master Volume", 1);
        MusicVolumePercent = PlayerPrefs.GetFloat("Music Volume", 1);
        SfxVolumePercent = PlayerPrefs.GetFloat("Sfx Volume", 1);

    }

    public void SetVolume(float Volume, AudioChannel Channel)
    {
        switch (Channel)
        {
            case AudioChannel.Master:
                MasterVolumePercent = Volume;
                break;
            case AudioChannel.Music:
                MusicVolumePercent = Volume;
                break;
            case AudioChannel.Sfx:
                SfxVolumePercent = Volume;
                break;
        }

        MusicSources[0].volume = MusicVolumePercent * MasterVolumePercent;
        MusicSources[1].volume = MusicVolumePercent * MasterVolumePercent;

        MusicSources[0].loop = true;
        MusicSources[1].loop = true;

        PlayerPrefs.SetFloat("Master Volume", MasterVolumePercent);
        PlayerPrefs.SetFloat("Music Volume", MusicVolumePercent);
        PlayerPrefs.SetFloat("Sfx Volume", SfxVolumePercent);
        PlayerPrefs.Save();

    }

    public void PlaySound(AudioClip Clip, Vector3 Position)
    {
        if(Clip != null)
        {
            AudioSource.PlayClipAtPoint(Clip, Position, SfxVolumePercent * MasterVolumePercent);
        }
    }

    public void PlaySound(string SoundName, Vector3 Position)
    {
        PlaySound(Library.GetClip(SoundName), Position);   
    }

    public void Play2DSound(string SoundName)
    {
        SoundSource2D.PlayOneShot(Library.GetClip(SoundName), SfxVolumePercent * MasterVolumePercent);
    }

    public void PlayMusic(AudioClip Music, float FadeDuration = 1)
    {
        MusicSourceIndex = 1 - MusicSourceIndex;
        MusicSources[MusicSourceIndex].clip = Music;
        MusicSources[MusicSourceIndex].Play();
        StartCoroutine(MusicCrossFade(FadeDuration));
    }

    IEnumerator MusicCrossFade(float Duration)
    {
        float Percentage = 0;

        while(Percentage < 1)
        {
            Percentage += Time.deltaTime * 1 / Duration;
            MusicSources[MusicSourceIndex].volume = Mathf.Lerp(0, MusicVolumePercent * MasterVolumePercent, Percentage);
            MusicSources[1 - MusicSourceIndex].volume = Mathf.Lerp(MusicVolumePercent * MasterVolumePercent, 0, Percentage);
            yield return null;
        }
    }

	// Use this for initialization
	void Start () {
    }
	
	// Update is called once per frame
	void Update () {
	    if(PlayerTransform != null)
        {
            AudioListenerTransform.position = PlayerTransform.position;
        }
	}
}
