using UnityEngine;
using TMPro;

public class EndScreenManager : MonoBehaviour
{
    [Header("Riferimenti UI")]
    public GameObject endScreenPanel;
    public TextMeshProUGUI endMessageText;

    public void ShowEndScreen(string timerValue)
    {
        if (endMessageText != null)
        {
            // Testo provvisorio: lo cambierai in futuro
            endMessageText.text =
                "Complimenti, hai lasciato le tue vincite e chiesto aiuto: " +
                "da qui comincia il vero percorso di guarigione!\n\n" +
                $"Hai impiegato {timerValue} per completare il labirinto.\n" +
                "Ignorando guadagni, ipotetiche scorciatoie e consigli invitanti " +
                "ci avresti impiegato: [tempo di riferimento da definire].\n\n" +
                "Questo è un testo di esempio. In futuro qui inseriremo un messaggio " +
                "più strutturato sulla pericolosità del gioco d'azzardo e sulle vie di uscita.";
        }

        if (endScreenPanel != null)
        {
            endScreenPanel.SetActive(true);
        }

        // Se vuoi proprio congelare tutto il gioco:
        Time.timeScale = 0f; 
    }
}