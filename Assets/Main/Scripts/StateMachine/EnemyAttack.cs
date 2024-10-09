using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAttack : MonoBehaviour
{
    [Header("Attack Settings")]
    public Transform currentTarget;
    public float AttackDelay = 3f;
    public float attackRange = 5f;
    public int attackDamage = 10;

    [Header("Animation")]
    public float attackAnimationDuration = 1.0f; // Should match your actual animation length
    public float damageDelay = 0.5f; // Time into attack animation when damage is dealt

    private NavMeshAgent AttackingAgent;
    private Animator animator;
    private float lastAttackTime;
    private bool isFollowing;
    private bool isAttacking = false;

    // Animator parameter hashes for better performance
    private static readonly int IsAttacking = Animator.StringToHash("IsAttacking");
    private static readonly int IsMoving = Animator.StringToHash("IsMoving");
    private static readonly int TriggerAttack = Animator.StringToHash("TriggerAttack");

    void Start()
    {
        AttackingAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        lastAttackTime = -AttackDelay;
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
        if (isFollowing && currentTarget != null)
        {
            float distanceToTarget = Vector3.Distance(transform.position, currentTarget.position);

            // If out of attack range, move towards the target
            if (distanceToTarget > attackRange)
            {
                AttackingAgent.isStopped = false;
                AttackingAgent.SetDestination(currentTarget.position);
                SetWalkingState();
            }
            else
            {
                // If within attack range, check for attacking
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
        return distanceToTarget <= attackRange && timeSinceLastAttack >= AttackDelay && !isAttacking;
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
        // Trigger attack animation
        animator.SetTrigger(TriggerAttack);

        // Wait for damageDelay before dealing damage
        yield return new WaitForSeconds(damageDelay);

        // Deal damage
        DealDamage();

        // Wait for the remaining animation duration
        float remainingDuration = attackAnimationDuration - damageDelay;
        if (remainingDuration > 0)
        {
            yield return new WaitForSeconds(remainingDuration);
        }

        // End attack state
        isAttacking = false;

        // Check if we should return to walking or idle
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

    void DealDamage()
    {
        if (currentTarget == null) return;

        float distanceToTarget = Vector3.Distance(transform.position, currentTarget.position);
        if (distanceToTarget <= attackRange)
        {
            Unit targetUnit = currentTarget.GetComponentInParent<Unit>();
            if (targetUnit != null)
            {
                targetUnit.TakeDamage(attackDamage);
            }
        }
    }

    // This method can be called by an animation event if you prefer
    public void OnAnimationDamageFrame()
    {
        DealDamage();
    }

    void SetAttackingState()
    {
        animator.SetBool(IsAttacking, true);
        animator.SetBool(IsMoving, false);
        animator.SetTrigger(TriggerAttack);
        Debug.Log("Setting Attacking State");
    }

    void SetWalkingState()
    {
        animator.SetBool(IsAttacking, false);
        animator.SetBool(IsMoving, true);
        Debug.Log("Setting Walking State");
    }

    void SetIdleState()
    {
        animator.SetBool(IsAttacking, false);
        animator.SetBool(IsMoving, false);
        Debug.Log("Setting Idle State");
    }

    // Visualization for attack range in editor
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}