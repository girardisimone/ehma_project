using UnityEngine;
using UnityEngine.SceneManagement;

public class FinalSceneLoader : MonoBehaviour
{
    [Header("Nome della scena da caricare")]
    public string sceneName = "EndScene";

    [Header("Opzionale: ferma timer se serve")]
    public TimerManager timerManager; // Qui dovremo trascinare il GameManager!

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Uscita raggiunta! Salvataggio tempo...");

            // 1. SALVATAGGIO TEMPO
            if (timerManager != null)
            {
                timerManager.StopTimer();
                string finalTime = timerManager.GetCurrentTimeString();
                
                // Salva nella memoria "FinalTime"
                PlayerPrefs.SetString("FinalTime", finalTime);
                PlayerPrefs.Save();
            }

            // 2. CAMBIO SCENA
            Time.timeScale = 1f;
            SceneManager.LoadScene(sceneName);
        }
    }
}