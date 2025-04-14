using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIBehaviour : MonoBehaviour
{
    public float maxSpeed, steeringMaxSpeed, stoppingDistance;

    [Header("Display Settings")] 
    [SerializeField] private bool areVectorsOnDisplay;

    #region Behaviours

    public void Seek(Vector3 target, Rigidbody rb)
    {
        var targetDirection = CalculateTargetDirection(target);
        
        var steeringDirection = CalculateSteeringDirection(targetDirection, rb.velocity);
        
        var finalDirection = CalculateFinalDirection(steeringDirection, rb.velocity);
        
        DisplayVectors(rb.velocity, targetDirection, steeringDirection);

        rb.velocity = CalculateFinalVelocity(finalDirection) * Arrive(target);
    }

    public void Flee(Vector3 target, Rigidbody rb)
    {
        var targetDirection = - CalculateTargetDirection(target);
        
        var steeringDirection = CalculateSteeringDirection(targetDirection, rb.velocity);
        
        var finalDirection = CalculateFinalDirection(steeringDirection, rb.velocity);
        
        DisplayVectors(rb.velocity, targetDirection, steeringDirection);

        rb.velocity = CalculateFinalVelocity(finalDirection);
    }

    private float Arrive(Vector3 target)
    {
        var sqrDistance = (target - transform.position).sqrMagnitude;
        if (sqrDistance > Mathf.Pow(stoppingDistance, 2)) 
            return 1;

        return (sqrDistance / Mathf.Pow(stoppingDistance, 2));
    }

    #endregion
    
    #region Calculations

    private Vector3 CalculateTargetDirection(Vector3 target)
    {
        return (target - transform.position).normalized * (maxSpeed * Time.fixedDeltaTime);
    }

    private Vector3 CalculateSteeringDirection(Vector3 targetDirection, Vector3 currentVelocity)
    {
        var steeringDirection = targetDirection - currentVelocity;

        return steeringDirection.sqrMagnitude > Mathf.Pow(steeringMaxSpeed, 2)
            ? steeringDirection.normalized * steeringMaxSpeed
            : steeringDirection;
    }

    private Vector3 CalculateFinalDirection(Vector3 steeringDirection, Vector3 currentVelocity)
    {
        return currentVelocity + steeringDirection;
    }

    private Vector3 CalculateFinalVelocity(Vector3 finalDirection)
    {
        return finalDirection.sqrMagnitude > Mathf.Pow(maxSpeed, 2)
            ? finalDirection.normalized * maxSpeed
            : finalDirection;
    }

    private void DisplayVectors(Vector3 currentVelocity, Vector3 targetDirection, Vector3 steeringDirection)
    {
        if (!areVectorsOnDisplay) return;
        
        Debug.DrawRay(transform.position, currentVelocity, Color.blue);
        Debug.DrawRay(transform.position, targetDirection, Color.green);
        Debug.DrawRay(transform.position + currentVelocity, steeringDirection * 10, Color.red);
    }

    #endregion
} 
