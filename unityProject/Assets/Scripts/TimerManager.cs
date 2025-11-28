using UnityEngine;
using TMPro; // Necessario per TextMeshPro

public class TimerManager : MonoBehaviour
{
    // Riferimento all'oggetto TimerText
    public TextMeshProUGUI timerText;

    // Variabile per il conteggio del tempo (in secondi)
    private float startTime = 0f;

    // Bandiera per sapere se il timer deve essere aggiornato
    private bool timerRunning = false;

    void Start()
    {
        // Avvia il timer non appena il gioco inizia
        StartTimer();
    }

    public void StartTimer()
    {
        startTime = Time.time; // Registra il tempo di avvio della scena
        timerRunning = true;
        Debug.Log("Cronometro avviato.");
    }

    // --- FUNZIONE MANCANTE CHE CAUSAVA L'ERRORE ---
    public void AddTimePenalty(float secondsToAdd)
    {
        // Spostando indietro l'inizio, il tempo trascorso aumenta
        startTime -= secondsToAdd;
        Debug.Log($"[TIMER] Penalità applicata: +{secondsToAdd} secondi.");

        // Aggiorniamo subito la grafica per dare feedback immediato
        UpdateTimerUI();
    }
    // ----------------------------------------------

    void Update()
    {
        if (timerRunning)
        {
            UpdateTimerUI();
        }
    }

    void UpdateTimerUI()
    {
        // Calcola il tempo trascorso dal momento di avvio
        float timeElapsed = Time.time - startTime;

        // Formattta il tempo in minuti, secondi e centesimi
        int minutes = (int)(timeElapsed / 60f);
        int seconds = (int)(timeElapsed % 60f);
        int milliseconds = (int)((timeElapsed * 100f) % 100f);

        string formattedTime = string.Format("{0:00}:{1:00}.{2:00}", minutes, seconds, milliseconds);

        if (timerText != null)
        {
            timerText.text = formattedTime;
        }
    }

    public void StopTimer()
    {
        timerRunning = false;
        if (timerText != null)
            Debug.Log($"Cronometro fermato. Tempo finale: {timerText.text}");
    }

    // Restituisce il tempo attuale formattato come stringa (per la schermata finale)
    public string GetCurrentTimeString()
    {
        if (timerText != null)
        {
            return timerText.text;
        }
        return "00:00.00";
    }
}
