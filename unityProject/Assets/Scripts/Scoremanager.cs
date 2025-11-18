using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    // 1. SINGLETON INSTANCE: Accessibile da ScoreManager.Instance in tutta la scena
    public static ScoreManager Instance;

    // Il contatore statico è gestito qui
    public static int GemCount = 0;

    public TextMeshProUGUI scoreText;

    private void Awake()
    {
        // Imposta l'istanza unica. Questo previene problemi di ricerca.
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            // Distrugge le copie duplicate se per errore ne crei più di una
            Destroy(gameObject);
            return;
        }

        // Resetta e inizializza il display
        GemCount = 0;
        UpdateScoreText(GemCount);
    }

    // Aggiorna il valore interno e la visualizzazione UI
    public void UpdateScoreText(int newScore)
    {
        if (scoreText != null)
        {
            GemCount = newScore;
            scoreText.text = "Score: " + GemCount.ToString();
            scoreText.ForceMeshUpdate();
            Debug.Log($"ScoreManager: UI aggiornata a {GemCount}");
        }
    }
}