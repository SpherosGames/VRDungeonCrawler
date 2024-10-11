using UnityEngine.AI;
using UnityEngine;

public abstract class EnemyMoveBaseState : MonoBehaviour
{
    protected Animator animator;
    protected NavMeshAgent agent;
    protected EnemyAttack enemyAttack;
    public virtual void EnterState(EnemyMoveStateManager enemy)
    {
        animator = enemy.GetComponent<Animator>();
        agent = enemy.GetComponent<NavMeshAgent>();
        enemyAttack = enemy.GetComponent<EnemyAttack>();
    }
    public abstract void UpdateState(EnemyMoveStateManager enemy);
}