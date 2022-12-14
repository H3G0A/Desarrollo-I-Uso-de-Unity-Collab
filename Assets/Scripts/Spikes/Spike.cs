using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spike : MonoBehaviour
{
    Animator animator;
    private void Start()
    {
        this.animator = GetComponent<Animator>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag.Equals("Player"))
        {
            animator.SetTrigger("spikesUp");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag.Equals("Player"))
        {
            animator.SetTrigger("spikesDown");
        }
    }
}
