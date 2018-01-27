using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerPlace : MonoBehaviour {
    public int NumberPlacesFoundBeforeActivation = 0;

    private bool used = false;
    public bool triggerOnEnter = true;

    public void OnTriggerEnter(Collider other)
    {
        if (triggerOnEnter)
        {
            SendEventPlace(other);
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if(!triggerOnEnter)
        {
            SendEventPlace(other);
        }
    }

    public void SendEventPlace(Collider other)
    {
        PlayerController controller = other.GetComponent<PlayerController>();
        if (controller && !used && InGameManager.GetSingleton.numberPlacesFound >= NumberPlacesFoundBeforeActivation)
        {
            print("Trigger place enter");
            controller.CmdEnterPlace();
            used = true;
        }
    }
}
