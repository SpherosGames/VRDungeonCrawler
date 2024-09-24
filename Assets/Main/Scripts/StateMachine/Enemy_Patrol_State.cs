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
            enemy.GetComponent<EnemyAttack>().enabled = true;
            enemy.GetComponent<EnemyWander>().enabled = false;
            enemy.SwitchState(enemy.AttackingState);
            

        }
        
    }
    public override void OnCollisionEnter(EnemyMoveStateManager enemy)
    {

    }
}
