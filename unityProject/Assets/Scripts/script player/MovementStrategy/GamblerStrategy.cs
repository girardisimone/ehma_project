using UnityEngine;
public class GamblerStrategy : IMovementStrategy
{
    private float nextRollTime = 0f;
    private float currentMultiplier = 1f;

    public Vector2 CalculateMovement(Vector2 input, float baseSpeed)
    {
        if (Time.time >= nextRollTime)
        {
            RollDice();
            nextRollTime = Time.time + 2f; // Tira i dadi ogni 2 secondi
        }

        return input * (baseSpeed * currentMultiplier);
    }

    private void RollDice()
    {
        // Genera un numero casuale tra 0 e 100
        int chance = Random.Range(0, 100);

        if (chance < 10) currentMultiplier = 0.2f;       // 10% Critico fallimento (Lentissimo)
        else if (chance < 40) currentMultiplier = 0.8f;  // 30% Lento
        else if (chance < 80) currentMultiplier = 1.2f;  // 40% Normale/Veloce
        else currentMultiplier = 2.5f;                   // 20% JACKPOT (Velocissimo)
        
        // Debug.Log($"Gambler Rolled: x{currentMultiplier}"); 
    }
}