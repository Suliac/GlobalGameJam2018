using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Top_character_controller : MonoBehaviour
{

    private bool aiming;
    private bool Degaine = true;
    public GameObject bullet;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        LookMouseCursor();

        if (Input.GetMouseButton(1))
        {
            aiming = true;
            if (Degaine)
            {
                SoundManager.GetSingleton.audioSources[6].Play();
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
        SoundManager.GetSingleton.audioSources[7].Play();
        GameObject balle = (GameObject)Instantiate(bullet, transform.position, transform.rotation);
        Rigidbody rb = balle.GetComponent<Rigidbody>();
        rb.AddForce(transform.forward * 100, ForceMode.Impulse);
        Destroy(balle, 1.0f);
    }
}
