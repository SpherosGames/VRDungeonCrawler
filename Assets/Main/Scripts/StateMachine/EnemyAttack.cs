using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAttack : MonoBehaviour
{
     NavMeshAgent AttackinAgent;
    public Transform currentTarget;
    
    void Start()
    {
        AttackinAgent = GetComponent<NavMeshAgent>();
    }

    public void StartAttacking(Transform target)
    {
        currentTarget = target;
        Debug.Log($"hurr durr im attacking you {currentTarget}");
        Debug.Log(currentTarget.transform.position);
        AttackinAgent.SetDestination(currentTarget.position);


    }

    void Update()
    {
        
    }
}
