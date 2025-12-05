using UnityEngine;
using TMPro; // NECESSARIO per modificare il testo
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class princess_interaction : MonoBehaviour
{
    [Header("Collegamenti UI")]
    public GameObject popupWindow;      // La finestra intera
    public TextMeshProUGUI messageText; // TRASCINA QUI l'oggetto testo del messaggio
    public GameObject buttonYes;        // TRASCINA QUI il pulsante "Sì"
    public GameObject buttonNo;         // TRASCINA QUI il pulsante "No"

    [Header("Testi")]
    [TextArea] public string domandaIniziale = "Ciao viaggiatore! Sembri perso... Vuoi un aiuto?";
    [TextArea] public string indizioFinale = "Segui le stelle e ti porteranno all'uscita.";

    [Header("Audio")]
    public AudioClip pathRevealSound;
    [Range(0f, 1f)]
    public float volumeSuono = 1f;

    private AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0f;

        if (popupWindow != null) popupWindow.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Quando entri, resetta la finestra allo stato iniziale
            ResetWindow();
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

    // Funzione per rimettere il testo e i bottoni come all'inizio
    private void ResetWindow()
    {
        if (messageText != null) messageText.text = domandaIniziale;
        if (buttonYes != null) buttonYes.SetActive(true);
        if (buttonNo != null) buttonNo.SetActive(true);
    }

    // --- FUNZIONE PER IL TASTO "SÌ" ---
    public void Accept()
    {
        // 1. Riproduci il suono
        if (pathRevealSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(pathRevealSound, volumeSuono);
        }

        // 2. Nascondi i pulsanti (così non si può cliccare di nuovo)
        if (buttonYes != null) buttonYes.SetActive(false);
        if (buttonNo != null) buttonNo.SetActive(false);

        // 3. Mostra l'indizio finale
        if (messageText != null) messageText.text = indizioFinale;

        // 4. Avvia la procedura finale (attendi e poi chiudi)
        StartCoroutine(FinishInteractionSequence());
    }

    // --- FUNZIONE PER IL TASTO "NO" ---
    public void Decline()
    {
        if (popupWindow != null) popupWindow.SetActive(false);
    }

    // Coroutine: aspetta, poi chiude e rimuove le tentazioni
    IEnumerator FinishInteractionSequence()
    {
        // Aspetta 4 secondi per far leggere il testo
        yield return new WaitForSeconds(4f);

        // Chiudi la finestra
        if (popupWindow != null) popupWindow.SetActive(false);

        // Disattiva Gemme e Portali (la tua logica originale)
        DisableTemptations();
    }

    // --- TUA FUNZIONE ORIGINALE ---
    private void DisableTemptations()
    {
        // 1. Trova e disattiva tutte le GEMME
        GameObject[] gems = GameObject.FindGameObjectsWithTag("Gem");
        foreach (GameObject gem in gems)
        {
            gem.SetActive(false);
        }
        Debug.Log($"Principessa: Rimossi {gems.Length} diamanti.");

        // 2. Trova e disattiva tutti i PORTALI
        // Nota: FindObjectsByType è per Unity 2023+, se usi versioni vecchie usa FindObjectsOfType
        PortalTeleporter[] portals = FindObjectsByType<PortalTeleporter>(FindObjectsSortMode.None);

        foreach (PortalTeleporter portal in portals)
        {
            if (portal != null) portal.gameObject.SetActive(false);
        }
        Debug.Log($"Principessa: Rimossi {portals.Length} portali.");

        // Qui potresti chiamare la funzione per mostrare le stelle, se ne hai una
        // ShowPath(); 
    }
}