using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletSpawner : Spawner
{
    [SerializeField] GameObject bullet;

    private void Start()
    {
        SetUp(bullet, 5, 2.0f);
    }

    public void SpawnBullet(Transform spawnTransform)
    {
        Rigidbody rb = Spawn(bullet, spawnTransform).GetComponent<Rigidbody>();
        rb.AddForce(spawnTransform.forward * 32f, ForceMode.Impulse);
        rb.AddForce(spawnTransform.up * 2f, ForceMode.Impulse);
    }
}
