using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class City_Ambient_Sound : MonoBehaviour
{

    private bool isplayed = true;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (isplayed)
        {
            SoundManager.GetSingleton.GetClipFromName("Ambient").Play();
            isplayed = false;
        }

    }
}
