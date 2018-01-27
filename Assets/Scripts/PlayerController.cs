using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerController : NetworkBehaviour
{
    public float CopSpeed = 2.0f;
    public GameObject copSpawn;
    public GameObject profilerSpawn;

    public bool isCop = false;
    private GameObject spriteObject;
    private float camY;
    public LayerMask WhatToHit;

    private bool aiming;
    public GameObject bullet;

    // Use this for initialization
    void Start()
    {
        camY = Camera.main.transform.position.y;
        spriteObject = transform.GetChild(0).gameObject;
        if (isServer)// Policier
            CopInit();
        else // Profiler
            ProfilerInit();

    }

    // Update is called once per frame
    void Update()
    {
        if (!isLocalPlayer)
            return;

        if (isCop)
        {
            CopInput();
            Camera.main.transform.position = new Vector3(transform.position.x, camY, transform.position.z);
        }
        else
        {
            ProfilerInput();
        }

        if (Input.GetMouseButton(1))
        {
            aiming = true;
            if (Input.GetMouseButtonDown(0))
            {
                Shoot();
            }
        }

        if (Input.GetMouseButtonUp(1))
        {
            aiming = false;
        }
    }


    #region Cop behaviour
    public void CopInput()
    {
        if (!aiming)
        {
            var vertMove = Input.GetAxisRaw("Vertical");
            var HorizMove = Input.GetAxisRaw("Horizontal");

            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            transform.position += new Vector3(HorizMove, 0, vertMove) * Time.deltaTime * CopSpeed;
        }

        LookMouseCursor();
    }

    public void Shoot()
    {
        GameObject balle = (GameObject)Instantiate(bullet, transform.position, transform.rotation);
        Rigidbody rb = balle.GetComponent<Rigidbody>();
        rb.AddForce(transform.forward * 100,ForceMode.Impulse);
        Destroy(balle, 1.0f);
    }

    void LookMouseCursor()
    {

        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Vector3 dir = Vector3.zero;

        if (Physics.Raycast(ray, out hit, 1000))
        {
            dir = new Vector3(hit.point.x, transform.position.y, hit.point.z);
            transform.LookAt(dir);
        }

    }


    public void CopInit()
    {
        isCop = true;
        Camera.main.transform.position = copSpawn.transform.position;
        //Camera.main.transform.parent = transform;
    }

    [Command]
    public void CmdEnterPlace()
    {
        if (!isServer)
            return;
        // normalement ici = policier
        InGameManager.GetSingleton.PlaceFound();

        // TODO : ce qu'il se passe pour le policier a ce moment
        RpcEnterPlace();
    }
    #endregion

    #region Profiler behaviour
    public void ProfilerInput()
    {

    }
    public void ProfilerInit()
    {
        isCop = false;
        Camera.main.transform.position = profilerSpawn.transform.position;
        transform.position = new Vector3(profilerSpawn.transform.position.x, -1000, profilerSpawn.transform.position.y);
    }

    [ClientRpc]
    public void RpcEnterPlace()
    {
        if (isServer)
            return; // already done by command

        print("Recu event place");

        // TODO : Envoyer des journaux
    }

    #endregion

}
