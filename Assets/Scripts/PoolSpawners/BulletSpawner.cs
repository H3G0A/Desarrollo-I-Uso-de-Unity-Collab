using UnityEngine;

public class BulletSpawner : Spawner
{
    [SerializeField] GameObject bullet;
    [SerializeField] Transform player;

    private void Start()
    {
        SetUp(bullet, 5, 2.0f);
    }

    public void SpawnBullet(Transform spawnTransform)
    {
        Vector3 direction = player.position - spawnTransform.position;

        GameObject go = Spawn(bullet, spawnTransform);
        go.GetComponent<BulletController>().SetBulletSpawner(this);
        Rigidbody rb = go.GetComponent<Rigidbody>();

        rb.velocity = Vector3.zero;
        //rb.AddForce(spawnTransform.forward * 32f, ForceMode.Impulse);
        rb.AddForce(direction * 1.5f, ForceMode.Impulse);
    }

    public void DeSpawn(GameObject go)
    {
        objectsPool.RecicleObject(bullet, go);
    }
}
