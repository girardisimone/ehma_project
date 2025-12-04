using UnityEngine;
using UnityEngine.SceneManagement; // Necessario per cambiare scena

public class GameUIManager : MonoBehaviour
{
    [Header("Impostazioni Scene")]
    [Tooltip("MenuScene")]
    public string menuSceneName = "MenuScene";

    // Funzione per il pulsante ESCI / MENU
    public void GoToMainMenu()
    {
        // IMPORTANTE: Se il gioco era in pausa o finito (TimeScale 0),
        // dobbiamo riattivare il tempo prima di cambiare scena,
        // altrimenti il Menu sarà bloccato!
        Time.timeScale = 1f;

        Debug.Log("Torno al Menu Principale...");
        SceneManager.LoadScene(menuSceneName);
    }
}
