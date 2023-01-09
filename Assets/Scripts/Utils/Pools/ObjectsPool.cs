using System.Collections.Generic;
using UnityEngine;

public class ObjectsPool : MonoBehaviour
{
    private Dictionary<int, Queue<GameObject>> pool = new Dictionary<int, Queue<GameObject>>();
    private Dictionary<int, GameObject> parents = new Dictionary<int, GameObject>();
    private bool hasBeenPreloaded = false;


    public void PreLoad(GameObject objectToPool, int poolSize)
    {
        if(!hasBeenPreloaded)
        {
            int id = objectToPool.GetInstanceID();

            GameObject parent = new GameObject();
            parent.name = objectToPool.name + " Pool";

            Debug.Log("#Pool ParentName: " + parent.name);
            Debug.Log("#Pool id: " + id);

            parents.Add(id, parent);

            pool.Add(id, new Queue<GameObject>());

            hasBeenPreloaded = true;
        }

        for (int i = 0; i < poolSize; ++i)
        {
            CreateObject(objectToPool);
        }

    }

    public GameObject GetObject(GameObject objectToPool)
    {
        int id = objectToPool.GetInstanceID();

        if (pool[id].Count == 0)
        {
            CreateObject(objectToPool);
        }

        GameObject go = pool[id].Dequeue();
        go.SetActive(true);

        return go;
    }

    private void CreateObject(GameObject objectToPool)
    {
        int id = objectToPool.GetInstanceID();

        GameObject go = Instantiate(objectToPool) as GameObject;
        go.transform.SetParent(GetParent(id).transform);
        go.SetActive(false);

        pool[id].Enqueue(go);
    }

    public void RecicleObject(GameObject objectToPool, GameObject objectToRecicle)
    {
        int id = objectToPool.GetInstanceID();

        pool[id].Enqueue(objectToRecicle);
        objectToRecicle.SetActive(false);
    }

    private GameObject GetParent(int parentID)
    {
        GameObject parent;

        parents.TryGetValue(parentID, out parent);

        return parent;
    }

    
}
