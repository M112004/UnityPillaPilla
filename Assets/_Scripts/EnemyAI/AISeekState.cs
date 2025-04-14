using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AISeekState : BaseState
{
    [SerializeField] private float seekMaxSpeed;
    [SerializeField] private float steeringMaxSpeed;
    public override void Construct()
    {
        aiBehaviour.maxSpeed = seekMaxSpeed;
        aiBehaviour.steeringMaxSpeed = steeringMaxSpeed;
    }

    public override void Transition()
    {
        if (m_enemyAIStateMotor.stateEnum == AIState.Seek) return;
        base.Transition();
    }

    public override void FixedUpdateState()
    {
        aiBehaviour.Seek(m_enemyAIStateMotor.target.position, m_enemyAIStateMotor.rb);
    }
}
