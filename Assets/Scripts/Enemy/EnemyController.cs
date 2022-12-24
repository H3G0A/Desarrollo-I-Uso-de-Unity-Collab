using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    [SerializeField] int health;


    NavMeshAgent agent;
    [SerializeField] Transform player;

    [SerializeField] LayerMask whatIsPlayer;

    //Patroling
    Vector3 walkPoint;
    bool walkPointSetted;

    //Attacking
    [SerializeField] float timeBetweenAttacks;
    bool alreadyAttack;

    //States
    [SerializeField] float sightRange;
    [SerializeField] float attackRange;
    bool playerInSightRange;
    bool playerInAttackRange;

    [SerializeField] List<Transform> endPoints;

    [SerializeField] GameObject projectile;

    [SerializeField] Transform canon;

    BulletSpawner bulletSpawner;
    private void Awake()
    {
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
            /*Rigidbody rb = Instantiate(projectile, canon.position, Quaternion.identity).GetComponent<Rigidbody>();



            rb.AddForce(canon.forward * 32f, ForceMode.Impulse);
            rb.AddForce(canon.up * 2f, ForceMode.Impulse);*/

            bulletSpawner.SpawnBullet(canon);

            alreadyAttack = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }

    private void ResetAttack()
    {
        alreadyAttack = false;
    }

    public void TakeDamage(int value)
    {
        health -= value;
        if(health <= 0)
        {
            Invoke(nameof(DestroyEnemy), 0.5f);
        }
    }

    private void DestroyEnemy()
    {
        Destroy(gameObject);
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
            AttackPlayer();
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
