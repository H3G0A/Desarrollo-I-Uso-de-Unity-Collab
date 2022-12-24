using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*
 * Borrar esto cuando el jugador pueda disparar y llamar a touch cuando choquen las balas con el jugador.
 */
public class ToEraseController : MonoBehaviour
{
    int index = 0;
    public List<Target> targets;


    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            targets[index++].Touch();
        }
    }
}
