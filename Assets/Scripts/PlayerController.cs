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
    }

    #region Cop behaviour
    public void CopInput()
    {
        var vertMove = Input.GetAxisRaw("Vertical");
        var HorizMove = Input.GetAxisRaw("Horizontal");

        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        transform.position += new Vector3(HorizMove, 0, vertMove) * Time.deltaTime * CopSpeed;

        LookMouseCursor();
    }

    void LookMouseCursor()
    {

        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, 100))
        {

            transform.LookAt(new Vector3(hit.point.x, transform.position.y, hit.point.z));

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
