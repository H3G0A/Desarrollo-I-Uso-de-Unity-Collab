using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
{
    bool hasBeenTouched;
    public event Action onTargetHitted;
    Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();

        animator.SetTrigger("appear");
    }
    public void TargetHitted()
    {
        if (onTargetHitted != null)
        {
            onTargetHitted();
        }
    }
    public bool HasBeenTouched
    {
        get { return hasBeenTouched; }
    }

    public void Touch()
    {
        Debug.Log("Target: " + this.name + " has been touched");
        hasBeenTouched = true;
        TargetHitted();
        animator.SetTrigger("dissapear");
    }
}
