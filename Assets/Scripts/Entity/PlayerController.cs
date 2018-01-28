﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerController : NetworkBehaviour
{
    public float CopSpeed = 2.0f;
    public GameObject copSpawn;
    public GameObject profilerSpawn;

    public List<GameObject> currentNews;

    public bool isCop = false;
    private float camY;
    private GameObject spriteObject;

    private bool isDragging = false;
    private int indexNewsDragging = -1;
    private Vector3 lastMousePosition;

    public LayerMask WhatToHit;

    private bool aiming;
    private bool marcheOK = true;
    Animator ani;

    // Use this for initialization
    void Start()
    {
        camY = Camera.main.transform.position.y;
        spriteObject = transform.GetChild(0).gameObject;
        currentNews = new List<GameObject>();
        ani = GetComponentInChildren<Animator>();

        if (isServer)// Policier
            CopInit();
        else // Profiler
            ProfilerInit();

    }

    // Update is called once per frame
    void Update()
    {
        if ((!isServer && !isLocalPlayer) || (isServer && isLocalPlayer))
        {
            if (isCop)
            {
                CopInput();
                Camera.main.transform.position = new Vector3(transform.position.x, camY, transform.position.z);
            }
            else
            {
                ProfilerInput();
            }
        }


    }


    #region Cop behaviour
    public void CopInput()
    {
        var vertMove = Input.GetAxisRaw("Vertical");
        var HorizMove = Input.GetAxisRaw("Horizontal");

        if (!aiming)
        {

            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            transform.position += new Vector3(HorizMove, 0, vertMove) * Time.deltaTime * CopSpeed;

            Transform leg = gameObject.transform.GetChild(0);

            if (vertMove != 0 || HorizMove != 0)
            {
                ani.SetBool("Walk", true);
            }
            else
            {
                ani.SetBool("Walk", false);
            }

            Vector3 dirleg = transform.position;
            if (HorizMove > 0)
            {
                Vector3 vec = new Vector3(leg.rotation.x + 90, leg.rotation.y, leg.rotation.z + 90);
                leg.rotation = Quaternion.Euler(vec);
                //dirleg += new Vector3(0, 0, 1);
            }
            else if (HorizMove < 0)
            {
                Vector3 vec = new Vector3(leg.rotation.x + 90, leg.rotation.y, leg.rotation.z + 90);
                leg.rotation = Quaternion.Euler(vec);
                //dirleg -= new Vector3(0, 0, 1);
            }

            if (vertMove > 0)
            {
                Vector3 vec = new Vector3(leg.rotation.x + 90, leg.rotation.y, leg.rotation.z);
                leg.rotation = Quaternion.Euler(vec);
                //dirleg += transform.up;
            }

            else if (vertMove < 0)
            {
                Vector3 vec = new Vector3(leg.rotation.x - 90, leg.rotation.y, leg.rotation.z);
                leg.rotation = Quaternion.Euler(vec);
                //dirleg -= transform.up;
            }

            if (HorizMove != 0 || vertMove != 0)
            {
                StartCoroutine ("bruit");
            }

            //spriteObject.transform.LookAt(dirleg);

        }
        else
        {
            ani.SetBool("Walk", false);
        }

        if (Input.GetMouseButton(1))
        {
            aiming = true;
        }

        if (Input.GetMouseButtonUp(1))
        {
            aiming = false;
        }

    }

    IEnumerator bruit()
    {
        if (marcheOK)
        {
            marcheOK = false;
            SoundManager.GetSingleton.GetClipFromName("Pas").Play();
            yield return new WaitForSeconds(.3f);
            marcheOK = true;
        }
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Building"))
        {
            print("lol");
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
        int placeNumber = InGameManager.GetSingleton.PlaceFound();

        // TODO : ce qu'il se passe pour le policier a ce moment
        RpcEnterPlace(placeNumber);
    }
    #endregion

    #region Profiler behaviour
    public void ProfilerInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (indexNewsDragging == -1) // viens de cliquer
            {
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out hit, 100))
                {
                    if (hit.collider.gameObject.CompareTag("News"))
                    {
                        string name = hit.collider.gameObject.name;

                        // Si on clique sur news
                        var newsInList = currentNews.FirstOrDefault(obj => obj.GetInstanceID() == hit.collider.gameObject.GetInstanceID());
                        if (newsInList)
                        {
                            currentNews.Remove(newsInList);
                            currentNews.Add(newsInList); // objectif : le mettre a la fin

                            indexNewsDragging = currentNews.LastIndexOf(newsInList);
                            lastMousePosition = new Vector3(hit.point.x, 0, hit.point.z);
                            //print("clique sur " + name + ", id " + indexNewsDragging + " nb list:" + currentNews.Count.ToString());
                            ReorderNewsByPlaceInList();
                        }

                    }
                    //transform.LookAt(new Vector3(hit.point.x, transform.position.y, hit.point.z));

                }
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            indexNewsDragging = -1;
        }

        if (indexNewsDragging > -1) // on est en train de drag
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit, 100))
            {
                var newPosMouse = new Vector3(hit.point.x, 0, hit.point.z);
                var diffPos = newPosMouse - lastMousePosition;

                //print("Dragging from " + lastMousePosition + " to " + newPosMouse);

                currentNews[indexNewsDragging].transform.position += diffPos;
                lastMousePosition = newPosMouse;
            }
        }

    }

    public void ProfilerInit()
    {
        isCop = false;
        Camera.main.transform.position = profilerSpawn.transform.position;
        transform.position = new Vector3(profilerSpawn.transform.position.x, -1000, profilerSpawn.transform.position.y);
    }

    public void ReorderNewsByPlaceInList()
    {
        // ex : si 3 news -> le dernier est au top -> y=3, puis y=2 ...
        int currentY = currentNews.Count;
        if (currentY > 0)
        {
            for (int i = currentY; i > 0; i--)
            {
                int index = i - 1;
                currentNews[index].transform.position = new Vector3(currentNews[index].transform.position.x, i, currentNews[index].transform.position.z);
            }
        }
    }

    [ClientRpc]
    public void RpcEnterPlace(int numberPlace)
    {
        if (isServer)
            return; // already done by command

        print("Recu event place");
        List<GameObject> newsToPop = new List<GameObject>(InGameManager.GetSingleton.GetNewsForPlace(numberPlace).newsPrefabs);
        List<GameObject> newsSpots = new List<GameObject>(InGameManager.GetSingleton.newsPopPoint);
        for (int i = 0; i < newsSpots.Count; i++)
        {
            GameObject newsJustPoped = Instantiate(newsToPop[i], newsSpots[i].transform.position, Quaternion.Euler(90, 0, 0), InGameManager.GetSingleton.profilerView.transform);
            currentNews.Add(newsJustPoped);
        }

        ReorderNewsByPlaceInList();
    }

    #endregion

}