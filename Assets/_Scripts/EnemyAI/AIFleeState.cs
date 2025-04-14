using UnityEngine;

public class AIFleeState : BaseState
{
    [SerializeField] private float fleeMaxSpeed;
    [SerializeField] private float steeringMaxSpeed;

    public override void Transition()
    {
        if(m_enemyAIStateMotor.stateEnum == AIState.Seek)
            m_enemyAIStateMotor.ChangeState(GetComponent<AISeekState>());
    }

    public override void FixedUpdateState()
    {
        
    }
}

