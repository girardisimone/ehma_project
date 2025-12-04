using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class EndSceneManager : MonoBehaviour
{
    [Header("Dove appare il personaggio")]
    public Transform characterSpawnPoint;

    [Header("Riferimenti UI")]
    public TextMeshProUGUI specificMessageText; // Il testo che cambia

    [Header("Configurazione Finali")]
    public EndingData[] endings; 

    [System.Serializable]
    public struct EndingData
    {
        public string characterName;       // Es. "Topo" (solo per ordine visivo)
        public GameObject characterPrefab; // Il prefab (es. NPC_boy)
        [TextArea(3,5)]
        public string message;             // Il testo specifico per questo personaggio
    }

    private void Start()
    {
        SetupScene();
    }

    void SetupScene()
    {
        // 1. RECUPERA SCELTA (Default 0 se non trova nulla)
        int index = PlayerPrefs.GetInt("SelectedCharacter", 0);

        // Sicurezza: se l'indice è fuori range, usa 0
        if (index < 0 || index >= endings.Length) index = 0;

        EndingData selectedEnding = endings[index];

        // 2. AGGIORNA TESTO
        if (specificMessageText != null)
        {
            specificMessageText.text = selectedEnding.message;
        }

        // 3. SPAWN PERSONAGGIO
        if (selectedEnding.characterPrefab != null && characterSpawnPoint != null)
        {
            // Istanzia il personaggio
            GameObject instance = Instantiate(selectedEnding.characterPrefab, characterSpawnPoint.position, Quaternion.identity);
            
            // Blocca rotazione
            instance.transform.rotation = Quaternion.identity;

            // === DISATTIVA MOVIMENTO E FISICA ===
            // Spegni script di input
            var moveScript = instance.GetComponent<NewPlayerMovement>();
            if (moveScript != null) moveScript.enabled = false;

            // Blocca fisica
            var rb = instance.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.bodyType = RigidbodyType2D.Kinematic;
                rb.linearVelocity = Vector2.zero; 
            }

            // === FORZA ANIMAZIONE CAMMINATA (SUL POSTO) ===
            var anim = instance.GetComponent<Animator>();
            if (anim != null)
            {
                // Cammina verso la camera (Giù)
                anim.SetFloat("InputY", -1f); 
                anim.SetFloat("InputX", 0f);
            }
        }
    }

    // --- FUNZIONI PULSANTI ---

    public void OnPlayAgain()
    {
        Time.timeScale = 1f; // Sblocca il tempo
        SceneManager.LoadScene("MenuScene"); // Assicurati che il nome sia esatto
    }

    public void OnExitGame()
    {
        Debug.Log("Uscita dal gioco!");
        Application.Quit();
    }
}