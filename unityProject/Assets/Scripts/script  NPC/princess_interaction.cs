using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class princess_interaction : MonoBehaviour
{
    [Header("Collegamenti UI")]
    public GameObject popupWindow;

    [Header("Audio")]
    public AudioClip pathRevealSound;
    [Range(0f, 1f)]
    public float volumeSuono = 1f;

    private AudioSource audioSource;
    private NewPlayerMovement currentPlayer;

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

    // --- FUNZIONE PER IL TASTO "SÌ" ---
    public void Accept()
    {
        // A. Riproduci il suono
        if (pathRevealSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(pathRevealSound, volumeSuono);
        }

        // B. Chiudi il popup
        if (popupWindow != null) popupWindow.SetActive(false);

        // C. RIVELA PERCORSO (Logica tua delle stelline)
        Debug.Log("Percorso rivelato!");
        // ShowPath(); // <--- Qui chiamerai la tua funzione per le stelle

        // D. RIMUOVI TENTAZIONI (Disattiva Gemme e Portali)
        DisableTemptations();
    }

    public void Decline()
    {
        if (popupWindow != null) popupWindow.SetActive(false);
    }

    // --- NUOVA FUNZIONE: DISATTIVA GEMME E PORTALI ---
    private void DisableTemptations()
    {
        // 1. Trova e disattiva tutte le GEMME
        // (Funziona solo se le tue gemme hanno il Tag "Gem")
        GameObject[] gems = GameObject.FindGameObjectsWithTag("Gem");
        foreach (GameObject gem in gems)
        {
            gem.SetActive(false);
        }
        Debug.Log($"Principessa: Rimossi {gems.Length} diamanti.");

        // 2. Trova e disattiva tutti i PORTALI
        // (Cerca tutti gli oggetti che hanno lo script PortalTeleporter)
        PortalTeleporter[] portals = FindObjectsByType<PortalTeleporter>(FindObjectsSortMode.None);

        foreach (PortalTeleporter portal in portals)
        {
            // Disattiviamo l'intero oggetto del portale
            portal.gameObject.SetActive(false);
        }
        Debug.Log($"Principessa: Rimossi {portals.Length} portali.");
    }
}