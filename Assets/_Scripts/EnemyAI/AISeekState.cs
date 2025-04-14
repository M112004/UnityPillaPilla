using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AISeekState : BaseState
{
    [SerializeField] private float seekMaxSpeed;
    [SerializeField] private float steeringMaxSpeed;
    // En AISeekState.cs y AIFleeState.cs
    public override void Construct()
    {
        // Configuración original
        aiBehaviour.maxSpeed = seekMaxSpeed; // o fleeMaxSpeed en AIFleeState
        aiBehaviour.steeringMaxSpeed = steeringMaxSpeed;

        // Esperar a que el enemigo toque el suelo antes de comenzar a moverse
        StartCoroutine(WaitForLanding());
    }

    private IEnumerator WaitForLanding()
    {
        // Espera hasta que el objeto esté cerca del suelo
        float groundLevel = 0f;
        float threshold = 0.1f;

        while (Mathf.Abs(transform.position.y - groundLevel) > threshold)
        {
            yield return null;
        }

        // Ahora que está en el suelo, podemos asegurarnos de que las restricciones son correctas
        if (m_enemyAIStateMotor.rb != null)
        {
            m_enemyAIStateMotor.rb.useGravity = false;
            m_enemyAIStateMotor.rb.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotation;
        }
    }

    public override void Transition()
    {
        if (m_enemyAIStateMotor.stateEnum == AIState.Seek) return;
        base.Transition();
    }

    public override void FixedUpdateState()
    {
        if (m_enemyAIStateMotor.target == null)
        {
            Debug.LogWarning("No target assigned to enemy!");
            // Buscar el jugador como respaldo
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                m_enemyAIStateMotor.target = player.transform;
            }
            return;
        }

        // El resto del código...
        aiBehaviour.Seek(m_enemyAIStateMotor.target.position, m_enemyAIStateMotor.rb);
    }
}
