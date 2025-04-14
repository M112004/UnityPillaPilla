using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AISeekState : BaseState
{
    [SerializeField] private float seekMaxSpeed;
    [SerializeField] private float steeringMaxSpeed;
    public override void Construct()
    {
        
    }

    public override void Transition()
    {
        if(m_enemyAIStateMotor.stateEnum == AIState.Flee)
            m_enemyAIStateMotor.ChangeState(GetComponent<AIFleeState>());
        /*if (m_enemyAIStateMotor.isPlayerOnSight) return;
        m_enemyAIStateMotor.ChangeState(GetComponent<AIPatrolState>());*/
    }

    public override void FixedUpdateState()
    {
        
    }
}
