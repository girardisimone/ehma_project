using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class EndSceneManager : MonoBehaviour
{
    [Header("Dove appare il personaggio")]
    public Transform characterSpawnPoint;

    [Header("Riferimenti UI")]
    public TextMeshProUGUI specificMessageText; 
    public TextMeshProUGUI timeStatsText; // <--- Qui collegheremo il nuovo testo

    [Header("Configurazione Finali")]
    public EndingData[] endings; 

    [System.Serializable]
    public struct EndingData
    {
        public string characterName;
        public GameObject characterPrefab;
        [TextArea(3,5)]
        public string message;
    }

    private void Start()
    {
        SetupScene();
    }

    void SetupScene()
    {
        // 1. PERSONAGGIO (Caricamento)
        int index = PlayerPrefs.GetInt("SelectedCharacter", 0);
        if (index < 0 || index >= endings.Length) index = 0;

        EndingData selectedEnding = endings[index];

        if (specificMessageText != null) specificMessageText.text = selectedEnding.message;

        if (selectedEnding.characterPrefab != null && characterSpawnPoint != null)
        {
            GameObject instance = Instantiate(selectedEnding.characterPrefab, characterSpawnPoint.position, Quaternion.identity);
            instance.transform.rotation = Quaternion.identity;

            // Disattiva movimento
            var moveScript = instance.GetComponent<NewPlayerMovement>();
            if (moveScript != null) moveScript.enabled = false;

            var rb = instance.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.bodyType = RigidbodyType2D.Kinematic;
                rb.linearVelocity = Vector2.zero; 
            }

            // Anima sul posto
            var anim = instance.GetComponent<Animator>();
            if (anim != null)
            {
                anim.SetFloat("InputY", -1f); 
                anim.SetFloat("InputX", 0f);
            }
        }

        // 2. TEMPO (Visualizzazione)
        if (timeStatsText != null)
        {
            string playerTime = PlayerPrefs.GetString("FinalTime", "00:00");
            string idealTime = "00:07:00"; 

            // these are attempts for better visualization
           /*timeStatsText.text = $" <color=black> TIME TAKEN: <color=red>{playerTime}</color>\n" +
                                 $" <color=black> TIME NEEDED: <color=green>{idealTime}</color>";*/
            /*timeStatsText.text = $" <color=black> Time taken: {playerTime}</color>\n" +
                                 $" <color=black> Time needed: {idealTime}</color>";*/
            timeStatsText.text = $"<color=black>Time taken: </color>" +
                                 $"<color=black>{playerTime}</color>\n\n" +
                                 $"<color=black>Time needed: </color>" +
                                 $"<color=black>{idealTime}</color>";
        }
    }

    public void OnPlayAgain()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MenuScene");
    }

    public void OnExitGame()
    {
        Debug.Log("Uscita dal gioco!");
        Application.Quit();
    }
}