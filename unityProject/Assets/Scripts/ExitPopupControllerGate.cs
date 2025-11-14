using UnityEngine;
using UnityEngine.Tilemaps;

public class ExitPopupController : MonoBehaviour
{
    [Header("Oggetti di gioco")]
    public Tilemap exitWallTilemap;           // La tilemap del muro di uscita
    public TimerManager timerManager;         // Script del timer
    public ScoreManager scoreManager;         // Script dello score
    public MonoBehaviour playerMovementScript; // Script che controlla il movimento del player
    public Collider2D exitTriggerCollider;    // Il BoxCollider2D di ExitTrigger (opzionale)

    private void OnEnable()
    {
        // Blocca il movimento del giocatore mentre il popup è aperto
        if (playerMovementScript != null)
        {
            playerMovementScript.enabled = false;
        }
    }

    private void OnDisable()
    {
        // Riattiva il movimento quando il popup viene chiuso
        if (playerMovementScript != null)
        {
            playerMovementScript.enabled = true;
        }
    }

    // CONTINUA: il giocatore resta nel labirinto
    public void OnContinueClicked()
    {
        // Chiude solo il popup, il resto continua
        gameObject.SetActive(false);
    }

    // CHIEDI AIUTO / ESCI: il giocatore esce dal labirinto
    public void OnExitClicked()
    {
        // 1. Ferma il timer
        if (timerManager != null)
        {
            timerManager.StopTimer();
        }

        // 2. Resetta lo score a 0
        if (scoreManager != null)
        {
            scoreManager.UpdateScoreText(0);
        }

        // 3. Apre il muro di uscita (rimuove le tile dalla Tilemap_ExitWall)
        if (exitWallTilemap != null)
        {
            exitWallTilemap.ClearAllTiles();
        }

        // 4. (Opzionale) Disattiva il trigger così il popup non si riapre
        if (exitTriggerCollider != null)
        {
            exitTriggerCollider.enabled = false;
        }

        // 5. Chiudi il popup
        gameObject.SetActive(false);
    }
}
