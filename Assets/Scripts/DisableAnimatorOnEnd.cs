using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableAnimatorOnEnd : MonoBehaviour
{
    private Animator animator;
    // Use this for initialization
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void DisableAnimator()
    {
        animator.enabled = false;
    }

    void Playsoundjournal()
    {
        SoundManager.GetSingleton.GetClipFromName("Journaux_pop_up").Play();
    }
}
