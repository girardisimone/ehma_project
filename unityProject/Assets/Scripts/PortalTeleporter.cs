using UnityEngine;

public class PortalTeleporter : MonoBehaviour
{
    // --- Configurazione del Portale (Impostare nell'Inspector) ---

    [Tooltip("Il Portale di destinazione a cui il giocatore verrà teletrasportato.")]
    public PortalTeleporter destinationPortal;

    // Variabile per evitare teletrasporti immediati di ritorno.
    private bool isPlayerInside = false;

    // --- Logica di Teletrasporto (Entrata) ---

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 1. Controlla il Tag (e che non sia già dentro)
        if (other.CompareTag("Player") && !isPlayerInside)
        {
            // Se il portale di destinazione è valido
            if (destinationPortal != null)
            {
                // Dici al portale di destinazione che il player sta arrivando
                destinationPortal.isPlayerInside = true;

                // Teletrasporta il giocatore al centro del portale di destinazione
                // Usiamo la posizione del Portale di Destinazione come nuovo punto
                other.transform.position = destinationPortal.transform.position;

                Debug.Log($"Teletrasporto riuscito da {gameObject.name} a {destinationPortal.gameObject.name}");
            }
            else
            {
                Debug.LogError("Il Portale di destinazione non è impostato per " + gameObject.name);
            }
        }
    }

    // --- Logica di Teletrasporto (Uscita) ---

    private void OnTriggerExit2D(Collider2D other)
    {
        // Quando il giocatore esce da UNO QUALSIASI dei trigger (o quello di arrivo o quello di partenza),
        // resetta la bandiera per consentire il prossimo teletrasporto.
        if (other.CompareTag("Player"))
        {
            isPlayerInside = false;
        }
    }
}