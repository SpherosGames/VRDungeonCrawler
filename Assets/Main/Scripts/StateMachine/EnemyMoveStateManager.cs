using UnityEngine;

public class EnemyMoveStateManager : MonoBehaviour
{
    public EnemyMoveBaseState CurrentState;
    public Enemy_Attacking_State AttackingState = new Enemy_Attacking_State();
    public Enemy_Idle_State IdleState = new Enemy_Idle_State();
    public Enemy_Patrol_State PatrolState = new Enemy_Patrol_State();

    public Animator animator;
    private EnemyAttack enemyAttack;
    void Start()
    {
       
        enemyAttack = GetComponent<EnemyAttack>();
        CurrentState = IdleState;
        CurrentState.EnterState(this);
    }
    private void Awake()
    {
        animator = GetComponent<Animator>();
        //Debug.Log(animator);
    }
    void Update()
    {
        CurrentState.UpdateState(this);
    }
    public void SwitchState(EnemyMoveBaseState state)
    {
        //Debug.Log($"Switching from {CurrentState.GetType().Name} to {state.GetType().Name}");
        CurrentState = state;
        state.EnterState(this);
    }
}