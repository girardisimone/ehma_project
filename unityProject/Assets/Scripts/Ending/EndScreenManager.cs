using UnityEngine;
using TMPro;

public class EndScreenManager : MonoBehaviour
{
    [Header("Riferimenti UI")]
    public GameObject endScreenPanel;
    public TextMeshProUGUI endMessageText;

    public void ShowEndScreen(string timerValue)
    {

        if (endScreenPanel != null)
        {
            endScreenPanel.SetActive(true);
        }

        // Se vuoi proprio congelare tutto il gioco:
        Time.timeScale = 0f; 
    }
}