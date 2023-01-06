using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    BulletSpawner bulletSpawner;
    [HideInInspector] public bool canMakeDamage;

    public void SetBulletSpawner(BulletSpawner bulletSpawner)
    {
        this.bulletSpawner = bulletSpawner;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag.Equals("Player"))
        {
            if(canMakeDamage)
            {
                collision.gameObject.GetComponent<PlayerController>().TakeDamage(1);
                bulletSpawner.DeSpawn(this.gameObject);
                canMakeDamage = false;
            }
        }
        //Add more tags if necessary
        else if (collision.gameObject.tag.Equals("Floor") || collision.gameObject.tag.Equals("Obstacle"))
        {
            bulletSpawner.DeSpawn(this.gameObject);
        }
    }

}
