using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAttack : MonoBehaviour
{
     NavMeshAgent AttackinAgent;
    public Transform currentTarget;
    GameObject enemy;
    
    void Start()
    {
        enemy = GetComponent<FieldOfView>().CurrentTarget;
        
        AttackinAgent = GetComponent<NavMeshAgent>();
    }

    public void StartAttacking(Transform target)
    {
        target = currentTarget;
       currentTarget = target;

        if (currentTarget != null)
        {
            AttackinAgent.SetDestination(currentTarget.position);
        }
        
        Debug.Log("is stopped" + AttackinAgent.isStopped);


    }

    void Update()
    {
        
    }
}
