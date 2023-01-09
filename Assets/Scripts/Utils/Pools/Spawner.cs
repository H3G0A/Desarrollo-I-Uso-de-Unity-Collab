using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    protected float timeToDespawn;
    [SerializeField] protected ObjectsPool objectsPool;
    protected void SetUp(GameObject primitive, int poolSize, float timeToDespawn)
    {
        this.timeToDespawn = timeToDespawn;
        objectsPool.PreLoad(primitive, poolSize);
    }

    protected GameObject Spawn(GameObject primitive, Transform spawnTransform)
    {
        GameObject go = objectsPool.GetObject(primitive);

        go.transform.position = spawnTransform.position;
        go.transform.rotation = Quaternion.identity;

        StartCoroutine(DeSpawn(primitive, go));

        return go;
    }

    IEnumerator DeSpawn(GameObject primitive, GameObject go)
    {
        yield return new WaitForSeconds(timeToDespawn);
        objectsPool.RecicleObject(primitive, go);
    }
}
