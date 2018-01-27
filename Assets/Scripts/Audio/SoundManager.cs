using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]

public struct ClipPerso{
    public string name;
    public AudioClip clip;
    public AudioSource source;

}
public class SoundManager : MonoBehaviour
{

    private static SoundManager instance;
    public static SoundManager GetSingleton
    {
        get { return instance; }
    }

    public void SetSingleton()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
            Destroy(gameObject);
    }

    private void Awake()
    {
        SetSingleton();
    }

    public ClipPerso[] Clips;
    //public AudioSource[] audioSources;

    public AudioSource GetClipFromName(string name)
    {
        return Clips.FirstOrDefault(c => c.name == name).source;
    }

    // Use this for initialization
    public void Start()
    {
        for (int i = 0; i < Clips.Length; i++)
        {
            GameObject child = new GameObject("Playersound");
            child.transform.parent = gameObject.transform;
            Clips[i].source = child.AddComponent<AudioSource>() as AudioSource;
            Clips[i].source.clip = Clips[i].clip;

        }
        //audioSources = new AudioSource[Clips.Length];
        //int i = 0;
        //while (i < Clips.Length)
        //{
        //    GameObject child = new GameObject("Player");

        //    child.transform.parent = gameObject.transform;

        //    audioSources[i] = child.AddComponent<AudioSource>() as AudioSource;

        //    audioSources[i].clip = Clips[i].clips;

        //    i++;
        //}
    }

    // Update is called once per frame
    void Update()
    {

    }
}