using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAttack : MonoBehaviour
{
    NavMeshAgent AttackinAgent;
    public Transform currentTarget;
    GameObject enemy;
    bool isFollowing;
    public float attackRange = 5f; // Set the desired distance to stop near the target

    void Start()
    {
        AttackinAgent = GetComponent<NavMeshAgent>();
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
    public void StartAttacking()
    {

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
            }
        }
    }
}
