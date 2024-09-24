using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Idle_State : EnemyMoveBaseState
{
    
    public override void EnterState(EnemyMoveStateManager enemy)
    {
        
    }
    public override void UpdateState(EnemyMoveStateManager enemy)
    {

    }
    public override void OnCollisionEnter(EnemyMoveStateManager enemy)
    {

    }
    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        
    }

}
