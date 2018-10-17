using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SoundLibrary : MonoBehaviour {

    public SoundGroup[] SoundGroups;
    Dictionary<string, AudioClip[]> GroupDictionary = new Dictionary<string, AudioClip[]>();

    public AudioClip GetClip(string Name)
    {
        if(GroupDictionary.ContainsKey(Name))
        {
            AudioClip[] Sounds = GroupDictionary[Name];
            return Sounds[Random.Range(0, Sounds.Length)];
        }
        return null;
    }

    [System.Serializable]
    public class SoundGroup
    {
        public string GroupName;
        public AudioClip[] Group;
    }

    void Awake()
    {
        foreach (SoundGroup CurrentGroup in SoundGroups)
        {
            GroupDictionary.Add(CurrentGroup.GroupName, CurrentGroup.Group);
        }
    }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
