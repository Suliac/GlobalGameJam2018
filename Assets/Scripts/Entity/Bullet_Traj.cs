using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet_Traj : MonoBehaviour
{

    public PlayerController source;
    public bool isLastShot = true;
    public Sprite RIP;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Pnj"))
        {
            print("touché");
            SoundManager.GetSingleton.GetClipFromName("Dead").Play();
            SpriteRenderer rend = collision.gameObject.GetComponent<SpriteRenderer>();
            Animator ani = collision.gameObject.GetComponentInChildren<Animator>();
            if (ani)
            {
                ani.enabled = false;
            }
            rend.sprite = RIP;

            Destroy(gameObject);
        }

        if (collision.gameObject.CompareTag("Boss"))
        {
            if (source)
            {
                SoundManager.GetSingleton.GetClipFromName("Dead").Play();
                SpriteRenderer rend = collision.gameObject.GetComponent<SpriteRenderer>();
                rend.sprite = RIP;
                Destroy(gameObject);
                source.CmdWin();
            }
        }

        if (isLastShot && !collision.gameObject.CompareTag("Boss"))
        {
            if (source)
            {
                Destroy(gameObject);
                source.CmdLoose();
            }
        }
    }
}
