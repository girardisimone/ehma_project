using UnityEngine;

public class BadNPCInteraction : MonoBehaviour
{
    [Header("Configurazione")]
    // IMPORTANTE: Seleziona qui quale dipendenza regala questo NPC
    public DependencyType dipendenzaOfferta;

    // Trascina qui il Canvas/Pannello del Popup
    public GameObject popupUI;

    private bool haGiaAccettato = false;

    void Start()
    {
        if (popupUI != null) popupUI.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Se tocca il player e non abbiamo ancora fatto il patto
        if (other.CompareTag("Player") && !haGiaAccettato)
        {
            if (popupUI != null) popupUI.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (popupUI != null) popupUI.SetActive(false);
        }
    }

    // --- COLLEGA QUESTO AL TASTO "SI" ---
    public void PlayerDiceSi()
    {
        // 1. Aggiungi la nuova dipendenza
        if (DependencyManager.Instance != null)
        {
            DependencyManager.Instance.AddDependency(dipendenzaOfferta);
        }

        // 2. Blocca l'NPC per il futuro
        haGiaAccettato = true;

        // 3. Chiudi tutto
        if (popupUI != null) popupUI.SetActive(false);
    }

    // --- COLLEGA QUESTO AL TASTO "NO" ---
    public void PlayerDiceNo()
    {
        // Chiudi solo il popup, così può riprovare se torna
        if (popupUI != null) popupUI.SetActive(false);
    }
}