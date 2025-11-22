using UnityEngine;

public class InternetPacketLossStrategy : IMovementStrategy
{
    private bool isLagging = false;
    private float nextLagTime = 0f;
    private float endLagTime = 0f;
    
    // Qui memorizziamo l'input premuto mentre siamo bloccati
    private Vector2 bufferedInput; 

    public Vector2 CalculateMovement(Vector2 input, float baseSpeed)
    {
        float currentTime = Time.time;

        // 1. GESTIONE TEMPISTICHE (Inizio e Fine Lag)
        if (!isLagging && currentTime >= nextLagTime)
        {
            // INIZIA IL LAG
            isLagging = true;
            
            // Durata richiesta: tra 0.5 e 2 secondi
            float duration = Random.Range(0.5f, 2.0f); 
            endLagTime = currentTime + duration;

            // Programmiamo il prossimo lag tra 3 e 6 secondi dopo la fine di questo
            nextLagTime = endLagTime + Random.Range(3f, 6f);
            
            // Resettiamo l'input memorizzato
            bufferedInput = Vector2.zero;
        }
        else if (isLagging && currentTime >= endLagTime)
        {
            // FINISCE IL LAG
            isLagging = false;
        }

        // 2. LOGICA MOVIMENTO
        if (isLagging)
        {
            // --- FASE CONGELAMENTO ---
            
            // Se il giocatore preme qualcosa mentre è bloccato, ce lo ricordiamo
            if (input != Vector2.zero)
            {
                bufferedInput = input;
            }

            // Restituisce zero: il personaggio è immobile
            return Vector2.zero;
        }
        else
        {
            // --- FASE RUBBER BAND (SCATTO) ---

            // Se abbiamo dell'input accumulato dal lag precedente
            if (bufferedInput != Vector2.zero)
            {
                // Creiamo una velocità molto alta (x5) nella direzione memorizzata
                Vector2 snapVelocity = bufferedInput * baseSpeed * 5f;

                // Riduciamo gradualmente il buffer verso zero.
                // Questo fa sì che lo scatto duri qualche frame (effetto scivolata veloce) 
                // invece di un solo frame istantaneo.
                bufferedInput = Vector2.MoveTowards(bufferedInput, Vector2.zero, Time.fixedDeltaTime * 10f);

                // Se il residuo è molto piccolo, lo azzeriamo del tutto per tornare normali
                if (bufferedInput.sqrMagnitude < 0.1f)
                {
                    bufferedInput = Vector2.zero;
                }

                return snapVelocity;
            }

            // Movimento assolutamente normale se non c'è lag e non c'è buffer
            return input * baseSpeed;
        }
    }
}