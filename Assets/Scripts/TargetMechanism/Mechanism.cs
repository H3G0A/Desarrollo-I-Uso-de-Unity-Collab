using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mechanism : MonoBehaviour
{
    [SerializeField] List<Target> targets;
    [SerializeField] Key key;

    /*
     * Cada uno de los targets se suscribe 
     * a la función
     */

    // Start is called before the first frame update
    void Start()
    {
        foreach (Target t in targets)
        {
            t.onTargetHitted += OnTargetHitted;
        }
    }

    private void OnTargetHitted()
    {
        Debug.Log("Event called");
        bool areAllTargetsHitted = true;
        foreach(Target t in targets)
        {
            if(!t.HasBeenTouched)
            {
                areAllTargetsHitted = false;
                break;
            }
        }

        if(areAllTargetsHitted)
        {
            ActionMechanism();
        }
    }

    private void ActionMechanism()
    {
        Debug.Log("Action Mechanism");
        key.gameObject.SetActive(true);
    }
}
