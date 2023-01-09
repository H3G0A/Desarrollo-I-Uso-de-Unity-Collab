using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Key : MonoBehaviour
{
    [SerializeField] private AudioClip keySound;
    Door doorSC;

    private void Start()
    {
        doorSC = GameObject.FindGameObjectWithTag("Door").GetComponent<Door>();
    }

    void OnTriggerEnter (Collider other){
        doorSC.addKeys(1);
        Destroy(gameObject,0.1f);
        AudioSource.PlayClipAtPoint(keySound, transform.position, 1);
        
    }
}
