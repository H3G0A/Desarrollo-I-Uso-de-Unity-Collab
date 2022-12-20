using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : HealthComponent
{
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Transform player;

    [SerializeField] LayerMask whatIsPlayer;

    //Patroling
    [SerializeField] Vector3 walkPoint;
    [SerializeField] bool walkPointSetted;

    //Attacking
    [SerializeField] float timeBetweenAttacks;
    [SerializeField] bool alreadyAttack;
    [SerializeField] int bulletDmg;
    [SerializeField] float bulletSpeed;
    BulletController bulletScript;

    //States
    [SerializeField] float sightRange;
    [SerializeField] float attackRange;
    [SerializeField] bool playerInSightRange;
    [SerializeField] bool playerInAttackRange;

    [SerializeField] List<Transform> endPoints;

    [SerializeField] GameObject projectile;

    [SerializeField] Transform canon;

    private void Start()
    {
        SetHealth();
        agent = GetComponent<NavMeshAgent>();
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

    private void ChasePlayer()
    {
        agent.SetDestination(player.position);
    }

    private void AttackPlayer()
    {
        agent.SetDestination(transform.position);

        transform.LookAt(player);

        if(!alreadyAttack)
        {
            bulletScript = Instantiate(projectile, canon.position, Quaternion.identity).GetComponent<BulletController>();
            bulletScript.velocity = bulletSpeed * canon.forward;
            bulletScript.dmg = bulletDmg;

            alreadyAttack = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }

    private void ResetAttack()
    {
        alreadyAttack = false;
    }

    private void Update()
    {
        //Check for sight and attack range
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

        if(playerInSightRange && !playerInAttackRange)
        {
            ChasePlayer();
        }
        else if(playerInSightRange && playerInAttackRange)
        {
            AttackPlayer();
        }
        else
        {
            Patroling();
        }

    }
}
