using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAttack : MonoBehaviour
{
     NavMeshAgent AttackinAgent;
    public Transform currentTarget;
    GameObject enemy;
    bool isfollowing;
    
    void Start()
    {   
        AttackinAgent = GetComponent<NavMeshAgent>();
    }

    public void StartAttacking(Transform target)
    {
       currentTarget = target;

        if (currentTarget != null)
        {
            isfollowing = AttackinAgent.SetDestination(currentTarget.position);
        }
       
    }

    void Update()
    {
        if (isfollowing)
        {
            AttackinAgent.SetDestination(currentTarget.position);
        }
    }
}
