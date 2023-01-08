using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : HealthComponent
{
    NavMeshAgent agent;
    [SerializeField] Transform player;

    [SerializeField] LayerMask whatIsPlayer;

    //Patroling
    Vector3 walkPoint;
    bool walkPointSetted;

    //Attacking
    [SerializeField] float timeBetweenAttacks;
    [SerializeField] bool alreadyAttack;
    [SerializeField] int bulletDmg;
    [SerializeField] float bulletSpeed;
    BulletController bulletScript;

    //States
    [SerializeField] float sightRange;
    [SerializeField] float attackRange;

    //AUDIO
    [SerializeField] AudioClip enemyShotSound;

    bool playerInSightRange;
    bool playerInAttackRange;

    [SerializeField] List<Transform> endPoints;

    [SerializeField] GameObject projectile;

    [SerializeField] Transform canon;

    BulletSpawner bulletSpawner;
    private void Awake()
    {
        SetHealth();
        agent = GetComponent<NavMeshAgent>();
        if(endPoints.Count == 0)
        {
            attackRange = sightRange;
        }
    }

    public void SetBulletSpawner(BulletSpawner bulletSpawner)
    {
        this.bulletSpawner = bulletSpawner;
    }

    private void SearchWalkPoint()
    {
        walkPoint = endPoints[Random.Range(0, endPoints.Count - 1)].position;
        walkPointSetted = true;
    }

    private void Patroling()
    {
        if(!walkPointSetted)
        {
            SearchWalkPoint();
        }
        else
        {
            agent.SetDestination(walkPoint);
        }

        Vector3 distaceToWalkPoint = transform.position - walkPoint;

        if(distaceToWalkPoint.magnitude < 1f)
        {
            walkPointSetted = false;
        }
    }


    private void AttackPlayer()
    {
        transform.LookAt(player);

        if(!alreadyAttack)
        {
            bulletSpawner.SpawnBullet(canon);
            audioSource.PlayOneShot(enemyShotSound);


            alreadyAttack = true;

            StartCoroutine(ResetAttack());
        }
    }

    IEnumerator ResetAttack()
    {
        yield return new WaitForSeconds(timeBetweenAttacks);
        alreadyAttack = false;
    }

    private void Update()
    {
        //Check for sight and attack range
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);
        
        if (playerInSightRange && playerInAttackRange)
        {
            agent.SetDestination(transform.position);
            AttackPlayer();
        }
        else if (playerInSightRange && !playerInAttackRange)
        {
            agent.SetDestination(player.position);
        }
        else
        {
            if(endPoints.Count > 0)
            {
                Patroling();
            }
        }

    }
}
