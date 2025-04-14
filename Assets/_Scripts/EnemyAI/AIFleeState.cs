using System.Collections;
using UnityEngine;

public class AIFleeState : BaseState
{
    [SerializeField] private float fleeMaxSpeed;
    [SerializeField] private float steeringMaxSpeed;

    // En AISeekState.cs y AIFleeState.cs
    public override void Construct()
    {
        // Configuración original
        aiBehaviour.maxSpeed = fleeMaxSpeed; // Usamos fleeMaxSpeed para la fase de Flee
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
        if(m_enemyAIStateMotor.stateEnum == AIState.Flee) return;
            base.Transition();
    }

    public override void FixedUpdateState()
    {
        if (m_enemyAIStateMotor.target == null)
        {
            Debug.LogWarning("No target assigned to enemy!");
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                m_enemyAIStateMotor.target = player.transform;
            }
            return;
        }

        // Usamos el método Flee para mover al enemigo en dirección contraria al jugador
        aiBehaviour.Flee(m_enemyAIStateMotor.target.position, m_enemyAIStateMotor.rb);
    }
}

