using UnityEngine;
using System.Collections;

public class MusicManager : MonoBehaviour {

    public AudioClip MainTheme;
    public AudioClip MenuTheme;

	// Use this for initialization
	void Start () {
        AudioManager.INSTANCE.PlayMusic(MainTheme, 2);
	}
	
	// Update is called once per frame
	void Update () {

	}
}
