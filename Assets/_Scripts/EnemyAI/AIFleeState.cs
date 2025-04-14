using UnityEngine;

public class AIFleeState : BaseState
{
    [SerializeField] private float fleeMaxSpeed;
    [SerializeField] private float steeringMaxSpeed;

    public override void Construct()
    {
        aiBehaviour.maxSpeed = fleeMaxSpeed;
        aiBehaviour.steeringMaxSpeed = steeringMaxSpeed;
    }

    public override void Transition()
    {
        if(m_enemyAIStateMotor.stateEnum == AIState.Flee) return;
            base.Transition();
    }

    public override void FixedUpdateState()
    {
        aiBehaviour.Flee(m_enemyAIStateMotor.target.position, m_enemyAIStateMotor.rb);
    }
}

