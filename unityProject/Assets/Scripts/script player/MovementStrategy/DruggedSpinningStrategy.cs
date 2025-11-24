using UnityEngine; // Necessario per Vector2, Quaternion, Time

public class DruggedSpinningStrategy : IMovementStrategy
{
    public Vector2 CalculateMovement(Vector2 input, float baseSpeed)
    {
        if (input == Vector2.zero) return Vector2.zero;

        // L'asse di movimento ruota di 90 gradi al secondo
        float angle = Time.time * 90f; 
        
        // Calcolo la rotazione
        Quaternion rotation = Quaternion.Euler(0, 0, angle);
        
        // Applico la rotazione all'input originale
        Vector2 rotatedInput = rotation * input;

        return rotatedInput * baseSpeed;
    }
}