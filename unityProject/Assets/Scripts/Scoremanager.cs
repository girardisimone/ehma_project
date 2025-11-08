using UnityEngine;
using TMPro; // *** IMPORTANTE: Devi importare la libreria TextMeshPro ***

public class ScoreManager : MonoBehaviour
{
    // Drag & Drop: Riferimento al tuo oggetto ScoreText nel pannello Inspector
    public TextMeshProUGUI scoreText;

    void Start()
    {
        // Inizializza il testo all'avvio del gioco
        UpdateScoreText(PlayerController.GemCount);
    }

    // Funzione pubblica chiamata dallo script PlayerController
    public void UpdateScoreText(int newScore)
    {
        // Controlla se il riferimento al testo è valido
        if (scoreText != null)
        {
            // Aggiorna la stringa di testo con il nuovo punteggio
            scoreText.text = "Score: " + newScore.ToString();
        }
    }
}