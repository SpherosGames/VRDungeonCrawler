using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Attacking_State : EnemyMoveBaseState
{
    public override void EnterState(EnemyMoveStateManager enemy)
    {
        enemy.GetComponent<EnemyWander>().enabled = false;
    }
    public override void UpdateState(EnemyMoveStateManager enemy)
    {

    }
    public override void OnCollisionEnter(EnemyMoveStateManager enemy)
    {

    }
}
