using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimationEvents : MonoBehaviour
{

    [SerializeField] EnemyController enemyController;

    public void ThrowBullet()
    {
        Debug.Log("#Event Throw bullet");
        enemyController.ThrowBullet();
    }

    public void ResetAttack()
    {
        enemyController.alreadyAttack = false;
    }
}
