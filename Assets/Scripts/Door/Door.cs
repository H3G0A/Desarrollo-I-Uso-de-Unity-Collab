using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public int keys = 0;
    [SerializeField] private int keysRequired = 6;
    [SerializeField] private float openAngle = -95f; //Ángulo de apertura de la puerta
    [SerializeField] private float closeAngle = 0.0f; //Ángulo de cierre de la puerta
    [SerializeField] private float speed = 3.0f; //Velocidad a la que se abre la puerta
    [SerializeField] private AudioClip openingSound;
    [SerializeField] private BoxCollider myCollider;
    [SerializeField] private BoxCollider myCollider2;

    [SerializeField] private bool isOpen = false; //Controla si la puerta está abierta o cerrada
    private bool sound = true;

    PlayerHUD playerHUDScr;
    AudioSource audioSource;
    private void Start()
    {
        playerHUDScr = GameObject.FindGameObjectWithTag("HUD").GetComponent<PlayerHUD>();
        playerHUDScr.SetKeys(keys);
        audioSource = GetComponent<AudioSource>();
    }

    void Update(){
        if(keys>=keysRequired){
            isOpen = true;
        }

        if(isOpen){
            Quaternion targetRotation = Quaternion.Euler(0, openAngle, 0);
            transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, speed * Time.deltaTime);
            myCollider.enabled = false;
            myCollider2.enabled = false;
            checkSound();
        }else{
            Quaternion targetRotation2 = Quaternion.Euler(0, closeAngle, 0);
            transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation2, speed * Time.deltaTime);
        }
        
    }

    private void checkSound(){
        if(sound){
            //audioSource.PlayOneShot(openingSound);
            sound = false;
        }
    }

    public void addKeys(int keys)
    {
        this.keys += keys;
        playerHUDScr.SetKeys(this.keys);
    }
    
}
