using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeDamage : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag.Equals("Player"))
        {
            other.GetComponent<PlayerController>().TakeDamage(1);
        }
    }
}
