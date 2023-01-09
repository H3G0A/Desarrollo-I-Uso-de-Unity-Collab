using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : HealthComponent
{
    NavMeshAgent agent;
    Transform player;

    [SerializeField] LayerMask whatIsPlayer;

    //Patroling
    Vector3 walkPoint;
    bool walkPointSetted;

    //Attacking
    public bool alreadyAttack;
    [SerializeField] int bulletDmg;
    [SerializeField] float bulletSpeed;

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

    bool playerOnVision = false;

    [SerializeField] Animator animator;

    private void Awake()
    {
        SetHealth();
        agent = GetComponent<NavMeshAgent>();
        if(endPoints.Count == 0)
        {
            attackRange = sightRange;
        }
    }

    public void Setup(BulletSpawner bulletSpawner, Transform player)
    {
        this.bulletSpawner = bulletSpawner;
        this.player = player;
    }

    private void SearchWalkPoint()
    {
        int index = Random.Range(0, endPoints.Count);
        walkPoint = endPoints[index].position;
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

    public void ThrowBullet()
    {
        bulletSpawner.SpawnBullet(canon);
        audioSource.PlayOneShot(enemyShotSound);
    }

    private void AttackPlayer()
    {
        transform.LookAt(player);

        alreadyAttack = true;

        animator.SetBool("Walking", false);
        animator.SetTrigger("Attack");

        agent.SetDestination(transform.position);
    }

    private void checkPlayerVisibility()
    {
        Vector3 direction = player.position - transform.position + Vector3.up;
        if (Physics.Raycast(transform.position, direction, out RaycastHit hit, attackRange))
        {
            if (hit.transform.tag.Equals("Player"))
            {
                playerOnVision = true;
            }
            else
            {
                playerOnVision = false;
            }
        }
    }

    private void Update()
    {
        //Check for sight and attack range
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);
        
        
        if(playerInSightRange || playerInAttackRange)
        {
            checkPlayerVisibility();
        }
        

        if (playerInSightRange && !playerInAttackRange && !alreadyAttack)
        {
            alreadyAttack = false;

            animator.SetBool("Walking", true);

            agent.SetDestination(player.position);
        }
        else if (playerInSightRange && playerInAttackRange && playerOnVision)
        {
            if(!alreadyAttack)
            {
                AttackPlayer();
            }
        }
        else
        {
            alreadyAttack = false;
            animator.SetBool("Walking", true);

            playerOnVision = false;

            if (endPoints.Count > 0)
            {
                Patroling();
            }
        }

    }
}
