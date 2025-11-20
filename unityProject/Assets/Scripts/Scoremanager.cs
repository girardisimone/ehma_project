using UnityEngine;
using TMPro;
using UnityEngine.UI; // <--- FONDAMENTALE PER LO SLIDER

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    [Header("Impostazioni")]
    [Tooltip("Il numero massimo di gemme che si possono accumulare.")]
    public int maxGems = 80;

    [Header("Riferimenti UI")]
    public TextMeshProUGUI scoreText;
    public Slider gemSlider; // <--- RIFERIMENTO ALLO SLIDER

    // Variabile privata interna
    private static int _gemCount = 0;

    // Proprietà pubblica che gestisce il limite (Clamp)
    public static int GemCount
    {
        get { return _gemCount; }
        set
        {
            // Blocca il valore tra 0 e maxGems (80)
            int limit = Instance != null ? Instance.maxGems : 80;
            _gemCount = Mathf.Clamp(value, 0, limit);
        }
    }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }

        GemCount = 0;
    }

    private void Start()
    {
        // Configurazione iniziale dello Slider
        if (gemSlider != null)
        {
            gemSlider.minValue = 0;
            gemSlider.maxValue = maxGems; // La barra sarà piena quando arrivi a 80
            gemSlider.value = 0;
        }

        // Aggiorniamo la UI all'avvio
        UpdateScoreText(0);
    }

    public void UpdateScoreText(int newScore)
    {
        // 1. Aggiorna il valore statico (Il blocco a 80 scatta qui automaticamente)
        GemCount = newScore;

        // 2. Aggiorna il TESTO
        if (scoreText != null)
        {
            scoreText.text = "Score: " + GemCount.ToString();
            scoreText.ForceMeshUpdate();
        }

        // 3. Aggiorna lo SLIDER
        if (gemSlider != null)
        {
            gemSlider.value = GemCount; // <-- QUESTA E' LA RIGA CHE MANCAVA
        }

        Debug.Log($"ScoreManager: Score aggiornato a {GemCount}");
    }
}