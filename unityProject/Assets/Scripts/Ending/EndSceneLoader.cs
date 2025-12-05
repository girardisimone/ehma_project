using UnityEngine;
using UnityEngine.SceneManagement;

public class FinalSceneLoader : MonoBehaviour
{
    [Header("Nome della scena da caricare")]
    public string sceneName = "EndScene";

    [Header("Opzionale: ferma timer se serve")]
    public TimerManager timerManager; // Trascina qui il Timer se vuoi fermarlo prima del cambio

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Controlla se è il Player a toccare l'uscita
        if (other.CompareTag("Player"))
        {
            Debug.Log("Uscita raggiunta! Caricamento scena finale...");

            // 1. Ferma il timer (se collegato) per bloccare il tempo finale
            if (timerManager != null)
            {
                timerManager.StopTimer();
            }

            // 2. Assicuriamoci che il tempo scorra (se era in pausa)
            Time.timeScale = 1f;

            // 3. Carica la scena finale
            SceneManager.LoadScene(sceneName);
        }
    }
}