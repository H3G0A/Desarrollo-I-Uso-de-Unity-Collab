using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    BulletSpawner bulletSpawner;

    public void SetBulletSpawner(BulletSpawner bulletSpawner)
    {
        this.bulletSpawner = bulletSpawner;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag.Equals("Player"))
        {
            collision.gameObject.GetComponent<PlayerController>().TakeDamage(1);
            bulletSpawner.DeSpawn(this.gameObject);
        }
        //Add more tags if necessary
        else if (collision.gameObject.tag.Equals("Floor") || collision.gameObject.tag.Equals("Obstacle"))
        {
            bulletSpawner.DeSpawn(this.gameObject);
        }
        else if(collision.gameObject.tag.Equals("Target"))
        {
            Debug.Log("Zasca!");
            collision.gameObject.GetComponent<Target>().Touch();
        }
    }

}
