using UnityEngine;
public class InternetAddictStrategy : IMovementStrategy
{
    private float nextLagTime = 0f;
    private bool isLagging = false;
    private float lagDuration = 0f;

    public Vector2 CalculateMovement(Vector2 input, float baseSpeed)
    {
        // Gestione del timer per il "Lag"
        if (Time.time >= nextLagTime)
        {
            if (isLagging)
            {
                // Finito il lag, decidiamo quando sarà il prossimo
                isLagging = false;
                nextLagTime = Time.time + Random.Range(2f, 3f); // Cammina bene per 2-5 secondi
            }
            else
            {
                // Inizia il lag
                isLagging = true;
                lagDuration = Random.Range(1f, 2f); // Si blocca per 1-2 secondi
                nextLagTime = Time.time + lagDuration;
            }
        }

        if (isLagging)
        {
            // Durante il lag, velocità zero (sta guardando il telefono)
            return Vector2.zero;
        }

        // Movimento normale quando non lagga
        return input * baseSpeed;
    }
}