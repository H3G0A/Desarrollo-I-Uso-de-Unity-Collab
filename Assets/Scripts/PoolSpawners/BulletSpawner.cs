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
        GameObject go = Spawn(bullet, spawnTransform);
        go.GetComponent<BulletController>().SetBulletSpawner(this);
        Rigidbody rb = go.GetComponent<Rigidbody>();

        rb.velocity = Vector3.zero;
        rb.AddForce(spawnTransform.forward * 32f, ForceMode.Impulse);
    }

    public void DeSpawn(GameObject go)
    {
        ObjectsPool.RecicleObject(bullet, go);
    }
}
