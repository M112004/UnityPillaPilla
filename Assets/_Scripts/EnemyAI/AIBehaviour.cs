using UnityEngine;

public class AIBehaviour : MonoBehaviour
{
    // Define el parámetro de velocidad; podría usarse o asignarse desde el estado
    public float maxSpeed = 0.1f;
    public float steeringMaxSpeed = 2f;  // Agrega la propiedad con un valor por defecto


    public void Seek(Vector3 destination, Rigidbody rb)
    {
        // Calcula la dirección normalizada entre la posición actual y el destino
        Vector3 direction = (destination - rb.position).normalized;
        // Calcula el movimiento con respecto al tiempo
        Vector3 movement = direction * maxSpeed * Time.fixedDeltaTime;
        // Mueve el Rigidbody; si usas física, evita modificar transform directamente
        rb.MovePosition(rb.position + movement);
    }

    public void Flee(Vector3 targetPosition, Rigidbody rb)
    {
        // Calcula la dirección contraria al jugador
        Vector3 fleeDirection = (rb.position - targetPosition).normalized;
        // Puedes ajustar la distancia multiplicando por un factor (por ejemplo, 5f)
        Vector3 fleeDestination = rb.position + fleeDirection * maxSpeed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + fleeDirection * maxSpeed * Time.fixedDeltaTime);
    }
}