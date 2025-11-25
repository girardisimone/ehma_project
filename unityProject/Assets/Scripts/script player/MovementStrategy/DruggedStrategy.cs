using UnityEngine;

public class DruggedStrategy : IMovementStrategy
{
    public Vector2 CalculateMovement(Vector2 input, float baseSpeed)
    {
        // Invece di scambiare x e y, invertiamo semplicemente il segno.
        // input.x positivo diventa negativo (e viceversa).
        // input.y positivo diventa negativo (e viceversa).
        Vector2 confusedInput = -input;
        
        // Restituiamo il vettore invertito mantenendo il rallentamento (0.7f)
        return confusedInput * (baseSpeed * 0.7f);
    }
}