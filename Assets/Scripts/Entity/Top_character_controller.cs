using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Top_character_controller : MonoBehaviour
{

    private bool aiming;
    private bool Degaine = true;
    public GameObject bullet;

    public int numberShoots = 3;
    Animator ani;

    // Use this for initialization
    void Start()
    {
        ani = GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (InGameManager.GetSingleton.State < GameState.Victory)
        {
            ani.SetBool("Walk", true);
            ani.SetBool("aim", false);
            LookMouseCursor();

            if (Input.GetMouseButton(1))
            {
                ani.SetBool("Walk", false);
                ani.SetBool("aim", true);
                aiming = true;
                if (Degaine)
                {
                    SoundManager.GetSingleton.GetClipFromName("Degaine").Play();
                    Degaine = false;
                }
                if (Input.GetMouseButtonDown(0))
                {
                    Shoot();
                }
            }

            if (Input.GetMouseButtonUp(1))
            {
                aiming = false;
                Degaine = true;
            }

            var vertMove = Input.GetAxisRaw("Vertical");
            var HorizMove = Input.GetAxisRaw("Horizontal");

            if (!aiming)
            {
                if (vertMove != 0 || HorizMove != 0)
                {
                    ani.SetBool("Walk", true);
                }
                else
                {
                    ani.SetBool("Walk", false);
                }
            }
            else
            {
                ani.SetBool("Walk", false);
            }
        }
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

    public void Shoot()
    {
        if (numberShoots > 0)
        {
            bool isLastBullet = numberShoots == 1;

            SoundManager.GetSingleton.GetClipFromName("Shoot").Play();
            GameObject balle = (GameObject)Instantiate(bullet, transform.position + transform.forward, transform.rotation);

            Bullet_Traj bulletScript = balle.GetComponent<Bullet_Traj>();
            bulletScript.source = GetComponentInParent<PlayerController>();
            bulletScript.isLastShot = isLastBullet;

            Rigidbody rb = balle.GetComponent<Rigidbody>();
            rb.AddForce(transform.forward * 50, ForceMode.Impulse);
            Destroy(balle, 1.0f);

            numberShoots--;

            InGameManager.GetSingleton.bullets[numberShoots].SetActive(false);
        }
    }
}
