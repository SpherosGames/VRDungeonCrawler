using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAttack : MonoBehaviour
{
    NavMeshAgent AttackinAgent;
    public Transform currentTarget;
    GameObject enemy;
    public float AttackDelay = 3f;
    bool isFollowing;
    public float attackRange = 5f;

    public int attackDamage = 10;
    public string attackAnimationTrigger = "Attack";

    private Animator animator;
    private float lastAttackTime;
    [SerializeField]private bool isAttacking = false;

    void Start()
    {
        AttackinAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        lastAttackTime = -AttackDelay; // Allow immediate first attack
    }

    public void GoToTarget(Transform target)
    {
        currentTarget = target;
        if (currentTarget != null)
        {
            isFollowing = true;
            AttackinAgent.SetDestination(currentTarget.position);
        }
    }

    public void Attacking()
    {
        if (CanAttack())
        {
            StartAttack();
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
        animator.SetBool("IsAttacking",true);
        lastAttackTime = Time.time;
    }

    // This method should be called by an Animation Event at the appropriate frame of the attack animation
    public void DealDamage()
    {
        float distanceToTarget = Vector3.Distance(transform.position, currentTarget.position);

        if (distanceToTarget <= attackRange)
        {
            Unit playerHealth = currentTarget.GetComponent<Unit>();
            if (playerHealth != null)
            {
                Debug.Log(playerHealth);
                playerHealth.TakeDamage(attackDamage);
            }
        }

        isAttacking = false;
    }

    void Update()
    {
        if (isFollowing && currentTarget != null)
        {
            float distanceToTarget = Vector3.Distance(transform.position, currentTarget.position);
            if (distanceToTarget > attackRange)
            {
                AttackinAgent.isStopped = false;
                AttackinAgent.SetDestination(currentTarget.position);
            }
            else
            {
                AttackinAgent.isStopped = true;
                Attacking(); 
            }
        }
    }
}