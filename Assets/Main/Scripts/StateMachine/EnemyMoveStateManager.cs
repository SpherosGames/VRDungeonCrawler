using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMoveStateManager : MonoBehaviour
{
    public EnemyMoveBaseState CurrentState;
    public Enemy_Attacking_State AttackingState = new Enemy_Attacking_State();
    public Enemy_Idle_State IdleState = new Enemy_Idle_State();
    public Enemy_Patrol_State PatrolState = new Enemy_Patrol_State();
    void Start()
    {
        CurrentState = PatrolState;

        CurrentState.EnterState(this);
    }


    void Update()
    {
        CurrentState.UpdateState(this);
    }
    public void SwitchState(EnemyMoveBaseState enemy)
    {
        CurrentState = enemy;
        enemy.EnterState(this);
    }


}

