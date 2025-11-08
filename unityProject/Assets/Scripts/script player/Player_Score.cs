using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Variabile statica per tenere traccia del punteggio
    // 'static' lo rende accessibile da qualsiasi altro script facilmente.
    public static int GemCount = 0;

    // Riferimento allo script che aggiorna la UI
    private ScoreManager scoreManager;

    void Start()
    {
        // Trova il ScoreManager all'inizio del gioco
        scoreManager = FindObjectOfType<ScoreManager>();
    }

    // Questa funzione viene chiamata quando il giocatore tocca qualcosa
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Controlla se l'oggetto toccato ha il tag "Gem"
        if (collision.CompareTag("Gem"))
        {
            // 1. Distruggi la gemma
            Destroy(collision.gameObject);

            // 2. Aumenta il conteggio
            GemCount++;

            // 3. Notifica lo ScoreManager di aggiornare la UI
            if (scoreManager != null)
            {
                scoreManager.UpdateScoreText(GemCount);
            }
        }
    }
}