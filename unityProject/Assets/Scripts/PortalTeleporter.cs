using UnityEngine;

public class PortalTeleporter : MonoBehaviour
{
    // --- Configurazione del Portale ---

    [Tooltip("Il Portale di destinazione a cui il giocatore verrà teletrasportato.")]
    public PortalTeleporter destinationPortal;

    [Tooltip("Il numero di gemme richieste per usare questo portale.")]
    public int travelCost = 5;

    // Variabile per evitare teletrasporti immediati di ritorno.
    private bool isPlayerInside = false;

    // --- Logica di Teletrasporto (Entrata) ---

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 1. Controlla il Tag e la bandiera anti-loop
        if (other.CompareTag("Player") && !isPlayerInside)
        {
            // Otteniamo il conteggio globale STATICO dal ScoreManager
            int currentGems = ScoreManager.GemCount;

            // ***** LOGICA DI PAGAMENTO *****

            if (currentGems >= travelCost)
            {
                // *** PAGAMENTO E TELETRASPORTO AUTORIZZATO ***

                // 2. Sottrai il costo dal contatore statico
                ScoreManager.GemCount -= travelCost;

                // 3. AGGIORNA LA UI (RICHIAMO AL SINGLETON INFALLIBILE)
                if (ScoreManager.Instance != null)
                {
                    ScoreManager.Instance.UpdateScoreText(ScoreManager.GemCount);
                    Debug.Log($"ScoreManager: UI aggiornata dopo il pagamento a {ScoreManager.GemCount}.");
                }

                // 4. Esegui il Teletrasporto e la Logica di Difficoltà
                if (destinationPortal != null)
                {
                    // 4a. TELETRASPORTO
                    destinationPortal.isPlayerInside = true;
                    other.transform.position = destinationPortal.transform.position;

                    // 4b. NOTIFICA IL GESTORE DELLA DIFFICOLTÀ
                    if (DifficultyManager.Instance != null)
                    {
                        DifficultyManager.Instance.RegisterPortalUse();
                    }

                    // 4c. DISTRUGGI PORTALE DI DESTINAZIONE
                    Destroy(destinationPortal.gameObject);

                    Debug.Log($"Pagamento di {travelCost} gemme riuscito. Teletrasporto completato.");
                }
                else
                {
                    Debug.LogError("Il Portale di destinazione non è impostato per " + gameObject.name);
                }
            }
            else
            {
                // *** PAGAMENTO RIFIUTATO ***
                Debug.Log($"Gemme insufficienti! Costo: {travelCost}, possedute: {currentGems}.");
            }
        }
    }

    // --- Logica di Teletrasporto (Uscita) ---

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Quando il giocatore esce, resetta la bandiera anti-loop.
            isPlayerInside = false;
        }
    }
}