using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    public Vector3 velocity;
    public int dmg;
    Rigidbody rb;
    BulletSpawner bulletSpawner;

    private void Awake()
    {
        Invoke(nameof(DestroyBullet), 2f);
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.AddForce(velocity, ForceMode.VelocityChange);
    }
    public void SetBulletSpawner(BulletSpawner bulletSpawner)
    {
        this.bulletSpawner = bulletSpawner;
    }

    private void DestroyBullet()
    {
        rb.detectCollisions = false;
        Destroy(this.gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            rb.velocity = Vector3.zero;
            DestroyBullet();
            collision.gameObject.GetComponent<PlayerController>().TakeDamage(dmg);
        }
    }

}
