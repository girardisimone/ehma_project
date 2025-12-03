using UnityEngine;
using System.Collections;

public class princess_interaction : MonoBehaviour
{
    [Header("Collegamenti UI")] public GameObject popupWindow;


    // Riferimento interno al player corrente
    private NewPlayerMovement currentPlayer;

    private void Start()
    {
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
            // currentPlayer = null; // Facoltativo: resetta il player se esce
        }
    }


}