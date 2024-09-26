using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy_Attacking_State : EnemyMoveBaseState
{
    public override void EnterState(EnemyMoveStateManager enemy)
    {
        NavMeshAgent agent = GetComponent<NavMeshAgent>();
       Animator animator = enemy.GetComponent<Animator>();
        animator.SetBool("IsWalking", true);

    }
    public override void UpdateState(EnemyMoveStateManager enemy)
    {

    }
   
}
