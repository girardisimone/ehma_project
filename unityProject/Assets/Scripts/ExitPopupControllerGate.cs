using UnityEngine;
using UnityEngine.Tilemaps;

public class ExitPopupControllerGate : MonoBehaviour
{
    [Header("Oggetti di gioco")]
    public Tilemap exitWallTilemap;            // La tilemap del muro di uscita
    public TimerManager timerManager;          // Script del timer
    public ScoreManager scoreManager;          // Script dello score
    public MonoBehaviour playerMovementScript; // Script che controlla il movimento del player
    public Collider2D exitTriggerCollider;     // Il BoxCollider2D di ExitTrigger (opzionale)

    private void OnEnable()
    {
        // Popup aperto → blocco il movimento
        if (playerMovementScript != null)
        {
            playerMovementScript.enabled = false;
        }
    }

    private void OnDisable()
    {
        // Popup chiuso → sblocco il movimento
        if (playerMovementScript != null)
        {
            playerMovementScript.enabled = true;
        }
    }

    // CONTINUA: il giocatore resta nel labirinto
    public void OnContinueClicked()
    {
        // non tocco timer, score, muro...
        gameObject.SetActive(false);   // chiudo solo il popup
    }

    // ESCI: il giocatore decide di uscire dal labirinto
    public void OnExitClicked()
    {
        // 1. Ferma il timer (come avevi prima)
        if (timerManager != null)
        {
            timerManager.StopTimer();
        }

        // 2. Azzera lo score (come avevi chiesto all’inizio)
        if (scoreManager != null)
        {
            scoreManager.UpdateScoreText(0);
        }

        // 3. Apre il muro di uscita (come prima)
        if (exitWallTilemap != null)
        {
            exitWallTilemap.ClearAllTiles();
        }

        // 4. Disattiva il trigger del popup così non si riapre
        if (exitTriggerCollider != null)
        {
            exitTriggerCollider.enabled = false;
        }

        // 5. Chiude il popup → ora il player può fisicamente uscire da D4
        gameObject.SetActive(false);
    }
}