using UnityEngine;
public class DruggedStrategy : IMovementStrategy
{
    public Vector2 CalculateMovement(Vector2 input, float baseSpeed)
    {
        // Scambio degli assi: X diventa Y, Y diventa X.
        // Aggiungiamo anche un effetto "rallentatore" (0.7f) per simulare stordimento.
        Vector2 confusedInput = new Vector2(input.y, input.x);
        
        return confusedInput * (baseSpeed * 0.7f);
    }
}
