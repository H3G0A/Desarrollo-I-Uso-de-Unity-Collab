using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemiesController : MonoBehaviour
{
    private List<EnemyController> enemies;
    [SerializeField] BulletSpawner bulletSpawner;
    [SerializeField] Transform player;

    void Awake()
    {
        enemies = new List<EnemyController>(GetComponentsInChildren<EnemyController>());
        foreach(EnemyController e in enemies)
        {
            e.Setup(bulletSpawner, player);
        }
    }
}
