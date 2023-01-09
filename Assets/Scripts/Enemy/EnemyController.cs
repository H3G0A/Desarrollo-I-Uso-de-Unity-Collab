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

    bool playerOnVision = false;
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
        Debug.Log("#Patrol index: " + index);
        walkPoint = endPoints[index].position;
        walkPointSetted = true;
        Debug.Log("#Patrol WalkPoint: " + walkPoint);
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

        //Debug.Log("#Patrol Distance To WalkPoint" + distaceToWalkPoint);

        if(distaceToWalkPoint.magnitude < 1f)
        {
            walkPointSetted = false;
        }
    }


    private void AttackPlayer()
    {
        transform.LookAt(player);

        /*
        * TODO
        */
        //Aplicar root motion

        if (!alreadyAttack)
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
        

        if (playerInSightRange && !playerInAttackRange)
        {
            /*
             * TODO
             */
            //Quitar root motion
            agent.SetDestination(player.position);
        }
        else if (playerInSightRange && playerInAttackRange && playerOnVision)
        {
            agent.SetDestination(transform.position);
            AttackPlayer();
        }
        else

        {
            /*
            * TODO
            */
            //Quitar root motion
            playerOnVision = false;
            if (endPoints.Count > 0)
            {
                Patroling();
            }
        }

    }
}
