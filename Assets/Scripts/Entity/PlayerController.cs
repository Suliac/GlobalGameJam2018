﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

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
    public bool KeyGet = false;
    Animator ani;

    public float countdown = 600.0f;
    public float alertCountdown = 150.0f;
    private float currentCountDown;
    private double lastSecond = 0;
    private Text textChrono;
    private bool needToLoose = false;
    private bool musicplayed = false;

    public GameObject missionOrder;
    // Use this for initialization
    void Start()
    {
        

        currentCountDown = countdown;
        camY = Camera.main.transform.position.y;
        spriteObject = transform.GetChild(0).gameObject;
        currentNews = new List<GameObject>();
        ani = GetComponentInChildren<Animator>();

        

        Camera.main.orthographicSize = 6f;
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
                if (InGameManager.GetSingleton.State < GameState.Victory)
                    CopInput();
                Camera.main.transform.position = new Vector3(transform.position.x, camY, transform.position.z);
            }
            else
            {
                if (InGameManager.GetSingleton.State < GameState.Victory)
                    ProfilerInput();
            }
        }
        else if (isLocalPlayer && !isServer)
        {
            currentCountDown -= Time.deltaTime;
            var currentSecond = Math.Truncate(currentCountDown);

            if (lastSecond != currentSecond)
            {
                if (currentCountDown <= 60)
                {
                    SoundManager.GetSingleton.GetClipFromName("Tick").Play();

                    if (!musicplayed)
                    {
                        SoundManager.GetSingleton.GetClipFromName("rush").Play();
                        musicplayed = true;
                    }
                }

                currentCountDown = Mathf.Max(currentCountDown, 0);
                TimeSpan t = TimeSpan.FromSeconds(currentCountDown);
                textChrono.text = string.Format("{0:D2}:{1:D2}", t.Minutes, t.Seconds);
            }

            lastSecond = currentSecond;

            if (InGameManager.GetSingleton.State < GameState.Victory && currentCountDown <= 0)
            {
                CmdLoose();
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

            Vector3 directionMove = new Vector3(HorizMove, 0, vertMove) * CopSpeed;
            CharacterController charaControl = GetComponent<CharacterController>();

            charaControl.Move(directionMove * Time.deltaTime);

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
                StartCoroutine("bruit");
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

        if (!musicplayed)
        {
            SoundManager.GetSingleton.GetClipFromName("Ambient").Play();
            StartCoroutine("Ambient2");
        }
     
    }


    IEnumerator Ambient2()
    {
        musicplayed = true;
        //SoundManager.GetSingleton.GetClipFromName("Ambient").Play();
        yield return new WaitForSeconds(50f);
        musicplayed = false;
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

    public void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("PickUp"))
        {
            KeyGet = true;
            print(KeyGet);
        }
    }

    public void CopInit()
    {
        isCop = true;
        Camera.main.transform.position = copSpawn.transform.position;
        InGameManager.GetSingleton.bulletPanel.SetActive(true);
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

    [Command]
    public void CmdWin()
    {
        if (!isServer)
            return;
        Win();
        RpcWin();
    }

    [ClientRpc]
    public void RpcWin()
    {
        if (isServer)
            return;

        Win();
    }

    public void Win()
    {
        InGameManager.GetSingleton.victoryPannel.SetActive(true);
        InGameManager.GetSingleton.State = GameState.Victory;
    }

    [Command]
    public void CmdLoose()
    {
        if (!isServer)
            return;
        Loose();
        RpcLoose();
    }

    [ClientRpc]
    public void RpcLoose()
    {
        if (isServer)
            return;

        Loose();
    }
    public void Loose()
    {
        InGameManager.GetSingleton.gameOverPanel.SetActive(true);
        InGameManager.GetSingleton.countdownPanel.SetActive(false);
        InGameManager.GetSingleton.bulletPanel.SetActive(false);
        InGameManager.GetSingleton.State = GameState.Loose;
    }
    #endregion

    #region Profiler behaviour
    public void SetAlertTimer()
    {
        currentCountDown = Mathf.Min(currentCountDown, alertCountdown);
    }

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
                    //print("Hit !");
                    if (hit.collider.gameObject.CompareTag("News"))
                    {
                        //print("Hit a news !");
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
            //print("reset");
        }

        if (indexNewsDragging > -1) // on est en train de drag
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit, 100))
            {
                var newPosMouse = new Vector3(hit.point.x, 0, hit.point.z);
                var diffPos = newPosMouse - lastMousePosition;

                //print("Dragging "+ currentNews[indexNewsDragging].name + " from " + lastMousePosition + " to " + newPosMouse);

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

        InGameManager.GetSingleton.countdownPanel.SetActive(true);
        textChrono = InGameManager.GetSingleton.countdownPanel.transform.GetChild(0).GetComponent<Text>();

        if (!isServer && !isLocalPlayer)
        {
            GameObject newsJustPoped = Instantiate(missionOrder, InGameManager.GetSingleton.newsPopPoint[0].transform.position, Quaternion.Euler(90, 0, 0), InGameManager.GetSingleton.profilerView.transform);
            currentNews.Add(newsJustPoped);
        }
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

        //print("Recu event place");
        List<GameObject> newsToPop = new List<GameObject>(InGameManager.GetSingleton.GetNewsForPlace(numberPlace).newsPrefabs);
        List<GameObject> newsSpots = new List<GameObject>(InGameManager.GetSingleton.newsPopPoint);
        for (int i = 0; i < newsToPop.Count; i++)
        {
            GameObject newsJustPoped = Instantiate(newsToPop[i], newsSpots[i].transform.position, Quaternion.Euler(90, 0, 0), InGameManager.GetSingleton.profilerView.transform);
            currentNews.Add(newsJustPoped);
        }

        ReorderNewsByPlaceInList();
    }

    #endregion

}