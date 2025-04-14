using System;
using UnityEngine;

[RequireComponent(typeof(EnemyAIStateMotor))]
public abstract class BaseState : MonoBehaviour/*, ITriggerTarget, ITriggerExitable*/
{
    [SerializeField] protected EnemyAIStateMotor m_enemyAIStateMotor;
    [SerializeField] protected AIBehaviour aiBehaviour;

    private void Awake()
    {
        m_enemyAIStateMotor = GetComponent<EnemyAIStateMotor>();
        aiBehaviour = GetComponent<AIBehaviour>();
    }

    public virtual void Construct() { }

    public virtual void Destruct() { }

    public virtual void Transition()
    {
        switch (m_enemyAIStateMotor.stateEnum)
        {
            case AIState.Flee:
                m_enemyAIStateMotor.ChangeState(GetComponent<AIFleeState>());
                break;
            case AIState.Seek:
                m_enemyAIStateMotor.ChangeState(GetComponent<AISeekState>());
                break;
            /*case AIState.Evade:
                m_enemyAIStateMotor.ChangeState(GetComponent<AIEvadeState>());
                break;
            case AIState.Pursue:
                m_enemyAIStateMotor.ChangeState(GetComponent<AIPursueState>());
                break;
            case AIState.FollowPath:
                m_enemyAIStateMotor.ChangeState(GetComponent<AIFollowPathState>());
                break;
            case AIState.Wander:
                m_enemyAIStateMotor.ChangeState(GetComponent<AIWanderState>());
                break;*/
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public virtual void UpdateState() { }

    public virtual void FixedUpdateState() { }

    /*public virtual void HitByPlayer(FPSPlayer player)
    {
        Debug.Log("Player hit by enemy");
        m_enemyAIStateMotor.player = player;
        m_enemyAIStateMotor.isPlayerOnSight = true;
    }

    public void ExitedByPlayer()
    {
        Debug.Log("Player exits enemy");
        m_enemyAIStateMotor.isPlayerOnSight = false;
    }*/
}
