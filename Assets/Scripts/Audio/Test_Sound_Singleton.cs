using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_Sound : MonoBehaviour {

	// Use this for initialization
	void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter(Collider other)
    {
        SoundManager.GetSingleton.audioSources[0].Play();
    }
}
