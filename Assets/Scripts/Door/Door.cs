using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] private int keys;
    [SerializeField] private bool isOpen = false;
    [SerializeField] private float openAngle = -95f;
    [SerializeField] private float closeAngle = 0.0f;
    [SerializeField] private float speed = 3.0f;

    [SerializeField] private AudioClip openingSound;

    void Update(){
        keys = Keys.keys;
        if(keys>=3){
            isOpen = true;
        }

        if(isOpen){
            Quaternion targetRotation = Quaternion.Euler(0, openAngle, 0);
            transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, speed * Time.deltaTime);
        }else{
            Quaternion targetRotation2 = Quaternion.Euler(0, closeAngle, 0);
            transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation2, speed * Time.deltaTime);
        }
    }

    private void OnTriggerExit(Collider other){
        if(other.tag == "TriggerDoor"){
            AudioSource.PlayClipAtPoint(openingSound, transform.position, 1);
        }
    }
}
