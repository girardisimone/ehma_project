using UnityEngine; 
// con questa strategia ogni cinque secondi viene estratta tra up, down, sx, dx
// una direzione fortunata

public class GamblerRouletteStrategy : IMovementStrategy
{
    private Vector2 luckyDir;   // La direzione che ti fa correre
    private Vector2 cursedDir;  // La direzione che ti rallenta
    private float nextShuffleTime = 0;

    public Vector2 CalculateMovement(Vector2 input, float baseSpeed)
    {
        // 1. GESTIONE TIMER: Gira la ruota ogni 5 secondi
        if (Time.time > nextShuffleTime)
        {
            ShuffleLuck();
            nextShuffleTime = Time.time + 5f;
        }

        // Se il giocatore è fermo, non calcoliamo nulla
        if (input == Vector2.zero) return Vector2.zero;

        float multiplier = 1f; // Velocità standard (x1)

        // 2. CALCOLO DELLA "SCOMMESSA"
        // Usiamo il Prodotto Scalare (Dot Product) per capire quanto l'input è simile alla direzione fortunata.
        // input.normalized serve per guardare solo la direzione, ignorando quanto forte premi la levetta.
        Vector2 inputDir = input.normalized;

        // CASO A: Sto andando nella direzione fortunata?
        // Vector2.Dot restituisce 1.0 se le direzioni sono identiche.
        // Restituisce ~0.7 se vai in diagonale (es. SU+DESTRA mentre la fortuna è SU).
        // Restituisce 0 se vai perpendicolare (es. DESTRA mentre la fortuna è SU).
        
        // Usiamo > 0.5f come soglia: significa che anche le diagonali valgono come "vittoria"!
        if (Vector2.Dot(inputDir, luckyDir) > 0.5f)
        {
            multiplier = 2.5f; // JACKPOT! (Velocità x2.5)
        }
        // CASO B: Sto andando nella direzione sfortunata?
        else if (Vector2.Dot(inputDir, cursedDir) > 0.5f)
        {
            multiplier = 0.4f; // SCONFITTA (Velocità ridotta al 40%)
        }

        // 3. APPLICAZIONE VELOCITÀ
        return input * (baseSpeed * multiplier);
    }

    private void ShuffleLuck()
    {
        // Array delle sole 4 direzioni cardinali
        Vector2[] possibleDirections = new Vector2[] 
        { 
            Vector2.up, 
            Vector2.down, 
            Vector2.left, 
            Vector2.right 
        };

        // Pesca una direzione a caso
        int randomIndex = Random.Range(0, possibleDirections.Length);
        luckyDir = possibleDirections[randomIndex];
        
        // La sfortuna è sempre l'opposto della fortuna
        cursedDir = -luckyDir;

        // Debug: Togli il commento sotto se vuoi vedere nella Console cosa esce
        // Debug.Log($"NUOVO GIRO! Fortuna: {luckyDir} | Sfortuna: {cursedDir}");
    }
}