﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pick_Up_Behaviour : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            SoundManager.GetSingleton.GetClipFromName("PickUp").Play();
            Destroy(gameObject);
        }
    }
}
