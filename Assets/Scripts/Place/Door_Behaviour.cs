using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door_Behaviour : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnTriggerEnter(Collider col)
    {
        var playercontroller = col.gameObject.GetComponent<PlayerController>();

        if (playercontroller)
        {
            if (playercontroller.KeyGet)
            {
                print("ouvert");
            }
        }
    }
}
