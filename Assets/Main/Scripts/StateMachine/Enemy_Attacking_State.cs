public class Enemy_Attacking_State : EnemyMoveBaseState
{
    public override void EnterState(EnemyMoveStateManager enemy)
    {
        base.EnterState(enemy);
        enemy.GetComponent<EnemyAttack>().enabled = true;
    }
    public override void UpdateState(EnemyMoveStateManager enemy)
    {
        // The EnemyAttack component will handle the attack logic and animations
        if (!enemy.GetComponentInChildren<FieldOfView>().HasTarget)
        {
            //enemy.GetComponent<EnemyAttack>().enabled = false;
            enemy.SwitchState(enemy.PatrolState);
        }
    }
}