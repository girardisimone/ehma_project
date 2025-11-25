using UnityEngine;
using UnityEngine.SceneManagement; // INDISPENSABILE per cambiare scena

public class MainMenuManager : MonoBehaviour
{
    // Inserisci qui il nome ESATTO della tua scena di gioco
    public string gameSceneName = "SampleScene";

    public void PlayGame()
    {
        // IMPORTANTE: Se il gioco precedente era finito con Time.timeScale = 0,
        // dobbiamo ripristinarlo a 1 altrimenti il gioco sarà bloccato all'avvio.
        Time.timeScale = 1f;

        // Carica la scena del labirinto
        SceneManager.LoadScene(gameSceneName);
    }

}