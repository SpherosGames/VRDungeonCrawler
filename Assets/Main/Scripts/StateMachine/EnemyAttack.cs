using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAttack : MonoBehaviour
{
    [Header("Attack Settings")]
    public Transform currentTarget;
    public Collider handCollider;  // Reference to the hand's collider
    float AttackDelay = 0.8f;
    float shieldBlockDuration = 1.5f; // New variable for shield block duration
    float attackRange = 5f;
    int attackDamage = 10;

    [Header("Animation")]
    float attackAnimationDuration = 1.0f;
    float damageDelay = 0.5f;

    private NavMeshAgent AttackingAgent;
    private Animator animator;
    private float lastAttackTime;
    private bool isFollowing;
    private bool isAttacking = false;
    [SerializeField] private bool isBlocked = false;

    // Animator parameter hashes for better performance
    private static readonly int IsAttacking = Animator.StringToHash("IsAttacking");
    private static readonly int IsMoving = Animator.StringToHash("IsMoving");
    private static readonly int TriggerAttack = Animator.StringToHash("TriggerAttack");

    void Start()
    {
        AttackingAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        lastAttackTime = -AttackDelay;

        if (handCollider != null)
        {
            handCollider.isTrigger = true;
            Debug.Log("Hand collider assigned and set as trigger.");
        }
        else
        {
            Debug.LogError("Hand collider not assigned.");
        }
    }

    public void GoToTarget(Transform target)
    {
        currentTarget = target;
        if (currentTarget != null)
        {
            isFollowing = true;
            AttackingAgent.SetDestination(currentTarget.position);
            SetWalkingState();
        }
    }

    void Update()
    {
        if (isBlocked)
        {
            //gameObject.GetComponent<Animator>().enabled = false;
            animator.Play("Idle");
        }
        if (!isBlocked)
        {
            //gameObject.GetComponent<Animator>().enabled = true;
        }
        if (isFollowing && currentTarget != null)
        {
            float distanceToTarget = Vector3.Distance(transform.position, currentTarget.position);

            if (distanceToTarget > attackRange)
            {
                AttackingAgent.isStopped = false;
                AttackingAgent.SetDestination(currentTarget.position);
                SetWalkingState();
            }
            else
            {
                AttackingAgent.isStopped = true;
                Attacking();
            }
        }
        else
        {
            SetIdleState();
        }
    }

    void Attacking()
    {
        if (CanAttack())
        {
            StartAttack();
        }
        else if (!isAttacking)
        {
            SetIdleState();
        }
    }

    bool CanAttack()
    {
        if (currentTarget == null) return false;

        float distanceToTarget = Vector3.Distance(transform.position, currentTarget.position);
        float timeSinceLastAttack = Time.time - lastAttackTime;
        return distanceToTarget <= attackRange && timeSinceLastAttack >= AttackDelay && !isAttacking && !isBlocked;
    }

    void StartAttack()
    {
        isAttacking = true;
        SetAttackingState();
        lastAttackTime = Time.time;
        AttackingAgent.isStopped = true;
        StartCoroutine(AttackSequence());
    }

    IEnumerator AttackSequence()
    {
        animator.SetTrigger(TriggerAttack);

        yield return new WaitForSeconds(damageDelay);

        // Enable hand collider to detect collision
        if (handCollider != null) handCollider.enabled = true;

        float remainingDuration = attackAnimationDuration - damageDelay;
        if (remainingDuration > 0)
        {
            yield return new WaitForSeconds(remainingDuration);
        }

        // Disable hand collider after the attack sequence
        if (handCollider != null) handCollider.enabled = false;

        isAttacking = false;
        if (currentTarget != null && Vector3.Distance(transform.position, currentTarget.position) > attackRange)
        {
            SetWalkingState();
            AttackingAgent.isStopped = false;
        }
        else
        {
            SetIdleState();
        }
    }

    public void Blocked()
    {
        Debug.Log("Blocked");
        isBlocked = true;
        animator.SetBool(IsAttacking, false); // Stop the attack animation
        StartCoroutine(BlockedDuration());
    }

    IEnumerator BlockedDuration()
    {
        // Stop the enemy from attacking for a short duration
        AttackingAgent.isStopped = true;
        SetIdleState();
        yield return new WaitForSeconds(shieldBlockDuration);
        isBlocked = false;
        AttackingAgent.isStopped = false;
    }

    void DealDamage()
    {
        if (currentTarget == null) return;

        Unit targetUnit = currentTarget.GetComponentInParent<Unit>();
        if (targetUnit != null)
        {
            targetUnit.TakeDamage(attackDamage);
        }
    }

    void SetAttackingState()
    {
        animator.SetBool(IsAttacking, true);
        animator.SetBool(IsMoving, false);
        animator.SetTrigger(TriggerAttack);
    }

    void SetWalkingState()
    {
        animator.SetBool(IsAttacking, false);
        animator.SetBool(IsMoving, true);
    }

    void SetIdleState()
    {
        animator.SetBool(IsAttacking, false);
        animator.SetBool(IsMoving, false);
    }

    // This method is called when the hand collider collides with another trigger collider
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Collided with: " + other.gameObject.name);
        if (other.CompareTag("Player") && isAttacking)
        {
            DealDamage();
        }
    }

    // Visualization for attack range in editor
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}