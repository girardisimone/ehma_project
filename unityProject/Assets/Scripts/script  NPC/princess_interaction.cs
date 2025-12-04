using UnityEngine;
using System.Collections;

// 1. NUOVO: Aggiunge il componente AudioSource automaticamente
[RequireComponent(typeof(AudioSource))]
public class princess_interaction : MonoBehaviour
{
    [Header("Collegamenti UI")]
    public GameObject popupWindow;

    // --- 2. NUOVO: VARIABILI AUDIO ---
    [Header("Audio")]
    public AudioClip pathRevealSound; // Trascina qui il suono "rivelazione percorso"
    [Range(0f, 1f)]
    public float volumeSuono = 1f;    // Regola il volume qui
    // --------------------------------

    private AudioSource audioSource;
    private NewPlayerMovement currentPlayer;

    private void Start()
    {
        // 3. NUOVO: Configurazione Audio
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0f; // Forza 2D (volume pieno)

        if (popupWindow != null) popupWindow.SetActive(false);
    }

    // --- GESTIONE TRIGGER ---
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            currentPlayer = other.GetComponent<NewPlayerMovement>();
            if (popupWindow != null) popupWindow.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (popupWindow != null) popupWindow.SetActive(false);
        }
    }

    // --- 4. NUOVO: FUNZIONE PER IL TASTO "SÌ" ---
    // Collega questo metodo al pulsante "Sì" nel tuo Popup
    public void Accept()
    {
        // A. Riproduci il suono
        if (pathRevealSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(pathRevealSound, volumeSuono);
        }

        // B. Chiudi il popup
        if (popupWindow != null) popupWindow.SetActive(false);

        // C. QUI METTI LA LOGICA PER MOSTRARE IL PERCORSO
        // Esempio: ShowPath();
        Debug.Log("Percorso rivelato! (Inserisci qui la tua logica del percorso)");
    }

    public void Decline()
    {
        // Chiudiamo semplicemente il popup
        if (popupWindow != null) popupWindow.SetActive(false);
    }
}