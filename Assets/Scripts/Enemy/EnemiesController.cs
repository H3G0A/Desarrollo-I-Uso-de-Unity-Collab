using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemiesController : MonoBehaviour
{
    private List<EnemyController> enemies;
    [SerializeField] BulletSpawner bulletSpawner;

    // Start is called before the first frame update
    void Awake()
    {
        enemies = new List<EnemyController>(GetComponentsInChildren<EnemyController>());
        foreach(EnemyController e in enemies)
        {
            e.SetBulletSpawner(bulletSpawner);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
