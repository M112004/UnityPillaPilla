using UnityEngine;


public enum AIState
{
    Flee,
    Seek
}

[RequireComponent(typeof(AIPatrolState),typeof(AIIdleState),typeof(AISeekState))]
[RequireComponent(typeof(AIFleeState),typeof(Rigidbody),typeof(Animator))]
public class EnemyAIStateMotor : MonoBehaviour
{
    [Header("State")] 
    public AIState stateEnum;
    
    public Animator anim;
    //public EnemyAIBehaviour enemyBehaviour;
    //public FPSPlayer player;
    public Transform target;
    public bool isPlayerOnSight, isIdleDone;
    public Rigidbody rb;

    private BaseState m_state;

    private void Awake()
    {
        //enemyBehaviour = GetComponent<EnemyAIBehaviour>();
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        m_state = GetComponent<AISeekState>();
    }

    private void Start()
    {
        m_state.Construct();
    }

    private void Update()
    {
        //if(!GameManager.Instance.isPaused)
        UpdateMotor();
    }

    private void FixedUpdate()
    {
        m_state.FixedUpdateState();
    }

    private void UpdateMotor()
    {
        m_state.Transition();
        //m_state.UpdateState();
    }

    public void ChangeState(BaseState newState)
    {
        m_state.Destruct();
        m_state = newState;
        m_state.Construct();
    }
}
