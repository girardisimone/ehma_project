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
        if (other.CompareTag("Player") && !isPlayerInside)
        {
            int currentGems = ScoreManager.GemCount;

            // ***** LOGICA DI PAGAMENTO *****

            if (currentGems >= travelCost)
            {
                // *** PAGAMENTO E TELETRASPORTO AUTORIZZATO ***

                // 1. Sottrai il costo
                ScoreManager.GemCount -= travelCost;

                // 2. AGGIORNA LA UI (RICHIAMO AL SINGLETON INFALLIBILE)
                if (ScoreManager.Instance != null)
                {
                    ScoreManager.Instance.UpdateScoreText(ScoreManager.GemCount);
                    Debug.Log($"ScoreManager: UI aggiornata dopo il pagamento a {ScoreManager.GemCount}.");
                }

                // 3. Esegui il Teletrasporto e la Distruzione
                if (destinationPortal != null)
                {
                    destinationPortal.isPlayerInside = true;
                    other.transform.position = destinationPortal.transform.position;

                    // CORREZIONE: Distruggi l'oggetto GameObject associato al Portale di Destinazione
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
            isPlayerInside = false;
        }
    }
}