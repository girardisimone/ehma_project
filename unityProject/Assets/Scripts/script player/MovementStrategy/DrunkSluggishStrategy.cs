using UnityEngine; // Necessario per Vector2, Mathf, Time

public class DrunkSluggishStrategy : IMovementStrategy
{
    private Vector2 currentVelocity;
    
    public Vector2 CalculateMovement(Vector2 input, float baseSpeed)
    {
        Vector2 targetVelocity = input * baseSpeed;
        
        // L'ubriaco ci mette tanto a partire (accelerazione bassa) 
        // e tantissimo a fermarsi (decelerazione quasi nulla, effetto scivoloso)
        float acceleration = (input.sqrMagnitude > 0) ? 2f : 0.5f; 
        
        // Interpoliamo (Lerp) per dare la sensazione di peso/inerzia
        // Nota: Moltiplichiamo per Time.fixedDeltaTime * 5f per renderlo frame-independent nel FixedUpdate
        currentVelocity = Vector2.MoveTowards(currentVelocity, targetVelocity, acceleration * Time.fixedDeltaTime * 5f);
        
        return currentVelocity;
    }
}