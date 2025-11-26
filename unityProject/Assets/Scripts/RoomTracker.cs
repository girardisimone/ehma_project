using UnityEngine;

public class RoomTracker : MonoBehaviour
{
    // Variabile per evitare di spammare la mappa 60 volte al secondo
    private bool playerInside = false;

    // Usiamo STAY invece di ENTER
    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Se la mappa non sa ancora che siamo qui, glielo diciamo
            if (!playerInside)
            {
                if (MiniMapController.Instance != null)
                {
                    Debug.Log("PLAYER RILEVATO IN: " + gameObject.name); // Debug per controllo
                    MiniMapController.Instance.UpdateMiniMap(gameObject.name);
                    playerInside = true;
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Quando esciamo, resettiamo il controllo
            playerInside = false;
        }
    }
}