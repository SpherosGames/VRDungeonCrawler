using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAttack : MonoBehaviour
{
    [Header("Attack Settings")]
    public Transform currentTarget;
    float AttackDelay = 0.8f;
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
        animator.SetTrigger(TriggerAttack);

        yield return new WaitForSeconds(damageDelay);

        DealDamage();
        
        float remainingDuration = attackAnimationDuration - damageDelay;
        if (remainingDuration > 0)
        {
            yield return new WaitForSeconds(remainingDuration);
        }

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

    public void OnAnimationDamageFrame()
    {
        DealDamage();
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

    // Visualization for attack range in editor
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}