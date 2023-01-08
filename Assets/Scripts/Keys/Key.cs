using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Key : MonoBehaviour
{
    [SerializeField] private AudioClip keySound;

    void OnTriggerEnter (Collider other){
        Keys.keys += 1;
        Destroy(gameObject,0.1f);
        AudioSource.PlayClipAtPoint(keySound, transform.position, 1);
        
    }
}
