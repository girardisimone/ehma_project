using UnityEngine;

public interface IMovementStrategy
{
    Vector2 CalculateMovement(Vector2 input, float baseSpeed);
}