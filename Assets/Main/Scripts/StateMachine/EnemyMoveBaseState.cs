using UnityEngine;

public abstract class EnemyMoveBaseState : MonoBehaviour
{
    public abstract void EnterState(EnemyMoveStateManager enemy);
    public abstract void UpdateState(EnemyMoveStateManager enemy);

}
