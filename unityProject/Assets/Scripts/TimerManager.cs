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

    void Update()
    {
        if (timerRunning)
        {
            // Calcola il tempo trascorso dal momento di avvio
            float timeElapsed = Time.time - startTime;

            // Formattta il tempo in minuti e secondi
            // Uso il casting a int per i minuti e i secondi interi
            int minutes = (int)(timeElapsed / 60f);
            int seconds = (int)(timeElapsed % 60f);

            // I millisecondi sono facoltativi, ma utili per precisione
            int milliseconds = (int)((timeElapsed * 100f) % 100f);

            // NUOVA RIGA: Formatta la stringa SENZA "Tempo:"
            // L'output sar�: 00:00.00
            string formattedTime = string.Format("{0:00}:{1:00}.{2:00}", minutes, seconds, milliseconds);


            // Aggiorna la visualizzazione UI
            if (timerText != null)
            {
                // Aggiorniamo il testo con la nuova stringa formattata
                timerText.text = formattedTime;
            }
        }
    }

    // [OPZIONALE] Puoi aggiungere qui un metodo per fermare il timer (es. se il gioco finisce)
    public void StopTimer()
    {
        timerRunning = false;
        Debug.Log($"Cronometro fermato. Tempo finale: {timerText.text}");
    }
    // Restituisce il tempo attuale formattato come stringa (quello mostrato a schermo)
    public string GetCurrentTimeString()
    {
        if (timerText != null)
        {
            return timerText.text;
        }
        return "";
    }
}
