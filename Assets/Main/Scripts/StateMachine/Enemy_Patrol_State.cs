using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class Enemy_Patrol_State : EnemyMoveBaseState
{
    Animator animator;
    public override void EnterState(EnemyMoveStateManager enemy) 
    {
        animator = GetComponent<Animator>();
        animator.SetBool("IsWalking", true);
       enemy.GetComponent<EnemyWander>().enabled = true;
    }
    public override void UpdateState(EnemyMoveStateManager enemy)
    {
        if (enemy.GetComponentInChildren<FieldOfView>().HasTarget)
        {
            enemy.GetComponent<EnemyAttack>().enabled = true;
            enemy.GetComponent<EnemyWander>().enabled = false;
            enemy.SwitchState(enemy.AttackingState);
            

        }
        
    }
    
}
