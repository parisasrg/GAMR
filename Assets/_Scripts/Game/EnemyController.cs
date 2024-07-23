using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    private NavMeshAgent agent;

    private GameObject player;
    private GameObject floor;
    public LayerMask groundlayer, playerlayer;

    EnemyStats enemyStat;
    Vector3 enemyForward;
    Vector3 playerPos;
    public float rotationSpeed = 5f;



    // Animation
    private Animator enemyAnimator;
    bool isWalking;
    bool isRunning;
    bool isAttacking = false;
    [SerializeField] 
    private float animationFinishTime = 0.9f;

    // Audio
    private AudioSource audioSource;
    [SerializeField]
    private List<AudioClip> growlClips;
    [SerializeField]
    private List<AudioClip> chaseClips;
    [SerializeField]
    private List<AudioClip> attackClips;

    bool readyPlay = true;

    private AudioClip currentEnemyGrowl;
    private AudioClip currentEnemyChase;
    private AudioClip currentEnemyAttack;

    // Patrol
    public Vector3 walkPoint;
    bool walkpointSet;
    public float walkPointRange = 1f;

    // Attack
    public float timeBetweenAttacks;
    bool alreadyAttacked;

    // States
    public float sightRange, attackRange;
    public bool playerInSightRange, playerInAttackRange;

    // Check if enemy moves
    bool isMoving = false;

    private void OnEnable() 
    {
        floor = PlayerManager.instance.floor;
        player = PlayerManager.instance.player;
        enemyStat = GetComponent<EnemyStats>();
        agent = GetComponent<NavMeshAgent>();
        agent.Warp(transform.position);

        enemyAnimator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        currentEnemyGrowl = RandomAudio(growlClips);
        currentEnemyChase = RandomAudio(chaseClips);
        currentEnemyAttack = RandomAudio(attackClips);
    }

    private void Update() 
    {
        playerInSightRange = Physics.CheckSphere(new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z), sightRange, playerlayer);
        playerInAttackRange = Physics.CheckSphere(new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z), attackRange, playerlayer);

        // Patrol State
        if(!playerInSightRange && !playerInAttackRange) Patrol();
        // Chase State
        if(playerInSightRange && !playerInAttackRange) Chase();
        // Attack State
        if(playerInSightRange && playerInAttackRange) Attack();       
    }

    private void Patrol()
    {
        // Walk Audio
        if(readyPlay)
        {
            readyPlay = false;
            audioSource.clip = currentEnemyGrowl;
            audioSource.Play();
            // audioSource.PlayOneShot(currentEnemyGrowl);
            Invoke(nameof(ResetPlay), 10);
        }

        if(!walkpointSet) SearchWalkPoint();

        if(walkpointSet)
        {
            agent.speed = 1f;
            agent.SetDestination(walkPoint);
        }     

        Vector3 distanceWalkPoint = transform.position - walkPoint;
        StartCoroutine(CheckMoving());

        // Walkpoint reached
        if(distanceWalkPoint.magnitude < 1f || !isMoving)
            walkpointSet = false;

        isRunning = false;
        AnimateZombie(); 
    }

    private void SearchWalkPoint()
    {
        // Calculate new point
        float randomX = Random.Range(-walkPointRange, walkPointRange);
        float randomZ = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y + 0.5f, transform.position.z + randomZ);

        if(Physics.Raycast(walkPoint, -transform.up, out RaycastHit hitInfo, 1f, groundlayer))
        {
            walkpointSet = true;
            walkPoint = hitInfo.point;
        }  
    }

    private void Chase()
    {
        // Chase Audio
        if(readyPlay)
        {
            readyPlay = false;
            audioSource.clip = currentEnemyChase;
            audioSource.Play();
            // audioSource.PlayOneShot(currentEnemyChase);
            Invoke(nameof(ResetPlay), 5);
        }

        agent.speed = 2f;
        agent.SetDestination(new Vector3(player.transform.position.x, floor.transform.position.y, player.transform.position.z));
        AnimateZombie(); 
    }

    private void Attack()
    {
        agent.SetDestination(transform.position);

        // Face player
        FaceTarget();


        // Attack Audio
        if(readyPlay)
        {
            readyPlay = false;
            audioSource.clip = currentEnemyAttack;
            audioSource.Play();
            // audioSource.PlayOneShot(currentEnemyAttack);
            Invoke(nameof(ResetPlay), 3);
        }

        if(!alreadyAttacked)
        {
            CharacterStats playerStats = player.GetComponent<CharacterStats>();

            if(playerStats != null && enemyStat.isDead == false && !isAttacking)
            {
                // For test
                // playerStats.TakeDamage(50);

                playerStats.TakeDamage(1);

                enemyAnimator.SetTrigger("IsAttacking");

                alreadyAttacked = true;
                Invoke(nameof(ResetAttack), timeBetweenAttacks);
            }
        }

        if(isAttacking && enemyAnimator.GetCurrentAnimatorStateInfo(1).normalizedTime >= animationFinishTime)
        {
            isAttacking = false;
        }
    }

    void FaceTarget ()
    {
        Vector3 direction = (player.transform.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
    }

    private void ResetAttack()
    {
        isAttacking = true;
        alreadyAttacked = false;
    }
    
    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z), sightRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z), attackRange);

        Gizmos.DrawRay(new Vector3(walkPoint.x, transform.position.y + 1f, walkPoint.z), -transform.up * 2f);
    }

    void AnimateZombie()
    {
        if(agent.speed >= 2)
        {
            enemyAnimator.SetBool("IsRunning", true);
            enemyAnimator.SetBool("IsWalking", false);
        }
        else if(agent.speed >= 1 && agent.speed < 2)
        {
            enemyAnimator.SetBool("IsRunning", false);
            enemyAnimator.SetBool("IsWalking", true);
        }
        else
        {
            enemyAnimator.SetBool("IsRunning", false);
            enemyAnimator.SetBool("IsWalking", false);
        }
    }

    private IEnumerator CheckMoving()
    {
        Vector3 startPos = transform.position;
        yield return new WaitForSeconds(1f);
        Vector3 finalPos = transform.position;
        if( startPos.x != finalPos.x || startPos.y != finalPos.y || startPos.z != finalPos.z)
            isMoving = true;
        else
            isMoving = false;
    }

    private AudioClip RandomAudio(List<AudioClip> audios)
    {
        int randIndex = Random.Range(0, audios.Count);
        AudioClip randomclip = audios[randIndex];

        return randomclip;
    }

    private void ResetPlay()
    { 
        readyPlay = true;
    }
}
