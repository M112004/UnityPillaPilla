using System.Collections;
using UnityEngine;

public class AIFleeState : BaseState
{
    [SerializeField] private float fleeMaxSpeed;  // Valor asignado desde Inspector (lo vamos a forzar a 2.5)
    [SerializeField] private float steeringMaxSpeed;

    // Flag para saber si el enemigo ya tocó el suelo
    private bool hasLanded = false;

    public override void Construct()
    {
        // Forzamos la velocidad a 2.5 (la mitad de la del jugador, que es 5)
        fleeMaxSpeed = 30f;
        aiBehaviour.maxSpeed = fleeMaxSpeed;
        aiBehaviour.steeringMaxSpeed = steeringMaxSpeed;

        // Reiniciamos el flag hasta confirmar que ha aterrizado
        hasLanded = false;

        // Esperar a que el enemigo toque el suelo antes de comenzar a moverse
        StartCoroutine(WaitForLanding());
    }

    private IEnumerator WaitForLanding()
    {
        float groundLevel = 0f;
        float threshold = 0.1f;

        // Espera hasta que el objeto esté cerca del suelo
        while (Mathf.Abs(transform.position.y - groundLevel) > threshold)
        {
            yield return null;
        }

        // Una vez en el suelo, desactivamos la gravedad y fijamos restricciones
        if (m_enemyAIStateMotor.rb != null)
        {
            m_enemyAIStateMotor.rb.useGravity = false;
            m_enemyAIStateMotor.rb.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotation;
        }

        // Indicamos que ya se ha aterrizado
        hasLanded = true;
    }

    public override void Transition()
    {
        if (m_enemyAIStateMotor.stateEnum == AIState.Flee)
            return;
        base.Transition();
    }

    public override void FixedUpdateState()
    {
        // Solo movemos al enemigo si ya ha aterrizado
        if (!hasLanded)
            return;

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

        // Calcula la dirección opuesta al jugador
        Vector3 directionAway = (m_enemyAIStateMotor.rb.position - m_enemyAIStateMotor.target.position).normalized;
        // Calcula el movimiento usando fleeMaxSpeed (2.5) y Time.fixedDeltaTime
        Vector3 movement = directionAway * aiBehaviour.maxSpeed * Time.fixedDeltaTime;
        // Mueve el Rigidbody
        m_enemyAIStateMotor.rb.MovePosition(m_enemyAIStateMotor.rb.position + movement);
    }
}
