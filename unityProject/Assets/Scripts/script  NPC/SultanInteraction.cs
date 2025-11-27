using UnityEngine;

public class SultanInteraction : MonoBehaviour
{
    [Header("Collegamenti")]
    // Qui trascinerai l'intero oggetto 'NPC_Sultan_popup'
    public GameObject popupWindow;

    private void Start()
    {
        // Per sicurezza, ci assicuriamo che il popup sia chiuso all'avvio
        if (popupWindow != null)
        {
            popupWindow.SetActive(false);
        }
    }

    // Quando il Player ENTRA nella zona del Sultano
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Controlliamo che sia proprio il Player (assicurati che il tuo player abbia il Tag "Player")
        if (other.CompareTag("Player"))
        {
            if (popupWindow != null)
            {
                popupWindow.SetActive(true); // Accende il popup
            }
        }
    }

    // Quando il Player ESCE dalla zona del Sultano
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (popupWindow != null)
            {
                popupWindow.SetActive(false); // Spegne il popup
            }
        }
    }
}