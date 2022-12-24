using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    protected float timeToDespawn;
    protected void SetUp(GameObject primitive, int amount, float timeToDespawn)
    {
        this.timeToDespawn = timeToDespawn;
        ObjectsPool.PreLoad(primitive, amount);
    }

    protected GameObject Spawn(GameObject primitive, Transform spawnTransform)
    {
        

        GameObject go = ObjectsPool.GetObject(primitive);

        go.transform.position = spawnTransform.position;
        go.transform.rotation = Quaternion.identity;

        StartCoroutine(DeSpawn(primitive, go));

        return go;
    }

    IEnumerator DeSpawn(GameObject primitive, GameObject go)
    {
        yield return new WaitForSeconds(timeToDespawn);
        ObjectsPool.RecicleObject(primitive, go);
    }
}
