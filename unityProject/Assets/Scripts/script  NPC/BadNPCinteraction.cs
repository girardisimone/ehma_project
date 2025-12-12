using UnityEngine;

public class BadNPCInteraction : MonoBehaviour
{
    [Header("Configurazione")]
    
    public DependencyType dipendenzaOfferta;

    
    public GameObject popupUI;

    private bool haGiaAccettato = false;

    void Start()
    {
        if (popupUI != null) popupUI.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        
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

   
    public void PlayerDiceNo()
    {
        // Chiudi solo il popup, così può riprovare se torna
        if (popupUI != null) popupUI.SetActive(false);
    }
}