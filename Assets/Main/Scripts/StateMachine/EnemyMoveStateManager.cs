using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMoveStateManager : MonoBehaviour
{
    public EnemyMoveBaseState CurrentState;
    public Enemy_Attacking_State AttackingState;
    public Enemy_Idle_State IdleState;
    public Enemy_Patrol_State PatrolState;
    
    public Animator animator;
    void Start()
    {
        CurrentState = IdleState;
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

