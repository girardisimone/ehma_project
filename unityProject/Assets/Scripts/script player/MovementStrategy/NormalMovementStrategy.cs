using UnityEngine;

// Assicurati che questa classe implementi l'interfaccia IMovementStrategy
public class NormalMovementStrategy : IMovementStrategy
{
    public Vector2 CalculateMovement(Vector2 input, float baseSpeed)
    {
        // Comportamento standard: input * velocità
        return input * baseSpeed;
    }
}