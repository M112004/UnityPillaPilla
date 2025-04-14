using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AISeekState : BaseState
{
    [SerializeField] private float seekMaxSpeed;
    [SerializeField] private float steeringMaxSpeed;

    // Flag que indica si ya ha aterrizado
    private bool hasLanded = false;

    public override void Construct()
    {
        // Configuración de velocidad para buscar al jugador
        aiBehaviour.maxSpeed = seekMaxSpeed;
        aiBehaviour.steeringMaxSpeed = steeringMaxSpeed;

        // Reiniciamos el flag de aterrizaje
        hasLanded = false;
        // Esperamos a que el enemigo toque el suelo antes de comenzar a moverse
        StartCoroutine(WaitForLanding());
    }

    private IEnumerator WaitForLanding()
    {
        float groundLevel = 0f;
        float threshold = 0.1f;

        // Espera hasta que la posición en Y esté cerca del suelo
        while (Mathf.Abs(transform.position.y - groundLevel) > threshold)
        {
            yield return null;
        }

        // Alineamos la posición al nivel del suelo
        Vector3 pos = transform.position;
        transform.position = new Vector3(pos.x, groundLevel, pos.z);

        // Desactivamos la gravedad y congelamos la posición Y y las rotaciones
        if (m_enemyAIStateMotor.rb != null)
        {
            m_enemyAIStateMotor.rb.useGravity = false;
            m_enemyAIStateMotor.rb.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotation;
        }

        // Indicamos que ya se ha aterrizado y se puede mover
        hasLanded = true;
    }

    public override void Transition()
    {
        if (m_enemyAIStateMotor.stateEnum == AIState.Seek) return;
        base.Transition();
    }

    public override void FixedUpdateState()
    {
        // Si el enemigo aún no ha aterrizado, no se mueve
        if (!hasLanded)
            return;

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

        // Se llama al método Seek del AIBehaviour para moverse hacia el jugador
        aiBehaviour.Seek(m_enemyAIStateMotor.target.position, m_enemyAIStateMotor.rb);
    }
}
