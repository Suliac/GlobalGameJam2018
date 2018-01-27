using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerController : NetworkBehaviour
{
    public GameObject policeSpawn;
    public GameObject profilerSpawn;
    // Use this for initialization
    void Start()
    {
        if (isServer)
        {
            // Policier
            print("Je suis un policier");
        }
        else
        {
            // Profiler
            print("Je suis un profiler");
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
