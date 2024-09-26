using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.AI;

public class Enemy_Idle_State : EnemyMoveBaseState
{
    Animator animator;
    NavMeshAgent agent;
    
    public override void EnterState(EnemyMoveStateManager enemy)
    {
        animator = GetComponent<Animator>();
        animator.SetBool("IsIdle", true);
    }
    public override void UpdateState(EnemyMoveStateManager enemy)
    {
        if (agent.hasPath == true)
        {
            animator.SetBool("IsIdle", false);
            enemy.SwitchState(enemy.AttackingState);
            
        }
    }
   
    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        
    }

}
