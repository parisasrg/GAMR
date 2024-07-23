using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    //Enemy controller
    Transform target;
    NavMeshAgent agent;
    CharacterCombat combat;
    Vector3 enemyForward;
    Vector3 playerPos;

    EnemyStats enemyStat;

    public float rotationSpeed = 5f;
    public float lookRadius = 10f;

    //Enemy animation
    private Animator animator;

    private bool isRunning = false;
    private bool isAttacking = false;
    [SerializeField] 
    private float animationFinishTime = 0.9f;

    //Enemy audio
    [SerializeField]
    private AudioClip spearClip;
    private AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        target = PlayerManager.instance.player.transform;
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        combat = GetComponent<CharacterCombat>();
        enemyStat = GetComponent<EnemyStats>();
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if(target)
        {
            float distance = Vector3.Distance(target.position, transform.position);

            if (distance <= lookRadius)
            {
                agent.SetDestination(target.position);

                if (distance <= agent.stoppingDistance)
                {
                    CharacterStats targetStats = target.GetComponent<CharacterStats>();
                    if(targetStats != null && enemyStat.isDead == false && !isAttacking)
                    {
                        // Attack target
                        enemyForward = transform.TransformDirection(Vector3.forward);
                        playerPos = (target.position - transform.position).normalized;
                        targetStats.damageDot = Vector3.Dot(enemyForward,playerPos);

                        animator.SetTrigger("isAttacking");
                        if(!audioSource.isPlaying)
                        {
                            audioSource.clip = spearClip;
                            audioSource.Play();
                        }                    
                        StartCoroutine(InitialiseAttack());
                        combat.Attack(targetStats);
                    } 
                    // Face Target
                    FaceTarget();
                }
            }
        }

        AnimateRun(transform.position);

        if(isAttacking && animator.GetCurrentAnimatorStateInfo(1).normalizedTime >= animationFinishTime)
        {
            isAttacking = false;
        }
    }

    IEnumerator InitialiseAttack()
    {
        yield return new WaitForSeconds(.1f);
        isAttacking = true;
    }

    void FaceTarget ()
    {
        Vector3 direction = (target.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
    }

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, lookRadius);
    }

    void AnimateRun(Vector3 m)
    {
        isRunning = (agent.velocity.magnitude > 0) ? true : false;
        animator.SetBool("isRunning", isRunning);
    }
}
