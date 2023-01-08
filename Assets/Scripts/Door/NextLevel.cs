using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NextLevel : MonoBehaviour
{
    [SerializeField] private int scene;

    private void OnTriggerEnter(Collider other){
        if(other.tag == "Player"){
            SceneManager.LoadScene(scene);
        }
    }
}
