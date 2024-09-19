using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class Enemy_Patrol_State : EnemyMoveBaseState
{

    public override void EnterState(EnemyMoveStateManager enemy) 
    {
       enemy.GetComponent<EnemyWander>().enabled = true;
    }
    public override void UpdateState(EnemyMoveStateManager enemy)
    {
        if (enemy.GetComponentInChildren<FieldOfView>().HasTarget)
        {
            Debug.Log("in the update state if statement ");
            enemy.SwitchState(enemy.AttackingState);
            enemy.GetComponent<EnemyAttack>().enabled = true;
            enemy.GetComponent<EnemyAttack>().StartAttacking(enemy.GetComponentInChildren<FieldOfView>().CurrentTarget.transform);
           enemy.GetComponent<EnemyWander>().enabled = false;

        }
        
    }
    public override void OnCollisionEnter(EnemyMoveStateManager enemy)
    {

    }
}
