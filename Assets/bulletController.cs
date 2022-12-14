using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bulletController : MonoBehaviour
{
    private void Awake()
    {
        Invoke(nameof(DestroyBullet), 2f);
    }

    private void DestroyBullet()
    {
        Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag.Equals("Player"))
        {
            collision.gameObject.GetComponent<PlayerController>().TakeDamage(1);
            DestroyBullet();
        }
    }

}
