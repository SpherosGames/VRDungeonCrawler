using System.Collections;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAttack : MonoBehaviour
{
    public Transform currentTarget;
    public float AttackDelay = 3f;
    public float attackRange = 5f;
    public int attackDamage = 10;
    public float damageDelay = 0.5f;

    private NavMeshAgent AttackinAgent;
    private Animator animator;
    private float lastAttackTime;
    private bool isFollowing;
    private bool isAttacking = false;

    private static readonly int IsAttacking = Animator.StringToHash("IsAttacking");
    private static readonly int IsMoving = Animator.StringToHash("IsMoving");
    private static readonly int TriggerAttack = Animator.StringToHash("TriggerAttack");

    void Start()
    {
        AttackinAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        lastAttackTime = -AttackDelay;
        //SetIdleState();
    }

    public void GoToTarget(Transform target)
    {
        currentTarget = target;
        if (currentTarget != null)
        {
            isFollowing = true;
            AttackinAgent.SetDestination(currentTarget.position);
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
                AttackinAgent.isStopped = false;
                AttackinAgent.SetDestination(currentTarget.position);
                SetWalkingState();  // Should set IsMoving to true
            }
            else
            {
                // If within attack range, check for attacking
                AttackinAgent.isStopped = true;
                Attacking();  // Handles attack state
            }
        }
        else
        {
            SetIdleState();  // If not following, go to idle
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
        float distanceToTarget = Vector3.Distance(transform.position, currentTarget.position);
        float timeSinceLastAttack = Time.time - lastAttackTime;
        return distanceToTarget <= attackRange && timeSinceLastAttack >= AttackDelay && !isAttacking;
    }

    void StartAttack()
    {
        isAttacking = true;
        SetAttackingState();
        lastAttackTime = Time.time;
        AttackinAgent.isStopped = true;
        StartCoroutine(DealDamageAfterDelay());
    }

    IEnumerator DealDamageAfterDelay()
    {
        yield return new WaitForSeconds(damageDelay);
        DealDamage();
        isAttacking = false;
        SetIdleState();
        AttackinAgent.isStopped = false;
    }

    void DealDamage()
    {
        float distanceToTarget = Vector3.Distance(transform.position, currentTarget.position);
        if (distanceToTarget <= attackRange && currentTarget != null)
        {
            Unit targetUnit = currentTarget.GetComponentInParent<Unit>();
            if (targetUnit != null)
            {
                targetUnit.TakeDamage(attackDamage);
            }

        }
    }

    void SetAttackingState()
    {
        animator.SetBool(IsAttacking, true);  // Set to attacking
        animator.SetBool(IsMoving, false);
        animator.SetTrigger(TriggerAttack);  // Trigger attack
        Debug.Log("Setting Attacking State");
    }

    void SetWalkingState()
    {
        animator.SetBool(IsAttacking, false);
        animator.SetBool(IsMoving, true);  // Set to walking
        Debug.Log("Setting Walking State");
    }

    void SetIdleState()
    {
        animator.SetBool(IsAttacking, false);
        animator.SetBool(IsMoving, false);  // Set to idle
        Debug.Log("Setting Idle State");
    }
}
