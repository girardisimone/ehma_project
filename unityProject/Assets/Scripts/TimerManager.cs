using UnityEngine;
using TMPro;

public class TimerManager : MonoBehaviour
{
    public TextMeshProUGUI timerText;
    private float startTime = 0f;
    private bool timerRunning = false;

    void Start()
    {
        StartTimer();
    }

    public void StartTimer()
    {
        startTime = Time.time;
        timerRunning = true;
    }

    // --- FUNZIONE AGGIUNTA ---
    public void AddTimePenalty(float secondsToAdd)
    {
        startTime -= secondsToAdd; // Sposta indietro l'inizio per aumentare il tempo trascorso
        Debug.Log($"[TIMER] Penalità: +{secondsToAdd} secondi.");
        UpdateTimerUI();
    }
    // -------------------------

    void Update()
    {
        if (timerRunning)
        {
            UpdateTimerUI();
        }
    }

    void UpdateTimerUI()
    {
        float timeElapsed = Time.time - startTime;

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
    }

    public string GetCurrentTimeString()
    {
        if (timerText != null) return timerText.text;
        return "00:00.00";
    }
}