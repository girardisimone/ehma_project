using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class PortalTeleporter : MonoBehaviour
{
    [Header("Impostazioni Portale")]
    public PortalTeleporter destinationPortal; // Portale di arrivo
    public int travelCost = 3;                    // Costo in gemme per usare il portale
    public enum PortalType { In, Out}
    public PortalType portalType = PortalType.In;
    private bool playerInside = false;
    private PortalManager portalManager;
    private void Start()
    {
        

        // Registrazione nel manager
        if (PortalManager.Instance != null)
        {
            PortalManager.Instance.RegisterPortal(this);
            portalManager = PortalManager.Instance;
        }
        else
            Debug.LogWarning($"Nessun PortalManager trovato per {name}");
    }
    
    private void Reset()
    {
        // Assicuriamoci che il collider sia un trigger
        var col = GetComponent<BoxCollider2D>();
        if (col != null)
            col.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Entra solo il player
        if (!other.CompareTag("Player")) return;

        // SE QUESTO è un portale di ARRIVO, non fare nulla
        if (portalType == PortalType.Out) return;

        playerInside = true;

        bool canUsePortal = ScoreManager.GemCount >= travelCost;

        if (PortalPopupController.Instance != null)
        {
            PortalPopupController.Instance.Show(this, canUsePortal, travelCost);
        }
        else
        {
            Debug.LogWarning("PortalPopupController non presente in scena.");
        }
    }


    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        // Anche qui: se è un portale di arrivo, ignoriamo
        if (CompareTag("portal_arrivo")) return;

        playerInside = false;

        if (PortalPopupController.Instance != null)
        {
            PortalPopupController.Instance.HideIfCurrent(this);
        }
    }

    /// <summary>
    /// Chiamato dal popup quando il giocatore sceglie di usare il portale.
    /// Scala le gemme e teletrasporta il player al portale di destinazione.
    /// </summary>
    public void TeleportPlayerAndPay()
    {
        if (!playerInside)
        {
            // Il player è uscito dal trigger nel frattempo
            Debug.Log("Il player non è più nel portale, teletrasporto annullato.");
            return;
        }
        
            // destinazione casuale badata su probabilità
        if (portalManager.isRandomConnections())
        {
            destinationPortal = portalManager.getRandomGrid().getDestinationPortal();
        }
        
        if (destinationPortal == null)
        {
            Debug.LogWarning("PortalTeleporter: destinationPortal non assegnato su " + name);
            return;
        }

        // Controllo gemme (di sicurezza)
        if (ScoreManager.GemCount < travelCost)
        {
            Debug.Log("Punteggio insufficiente per usare il portale.");
            return;
        }

        // Scala le gemme e aggiorna la UI
        int newScore = ScoreManager.GemCount - travelCost;
        ScoreManager.Instance.UpdateScoreText(newScore);  // Aggiorna lo score in UI

        // Trova il player e teletrasportalo
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            player.transform.position = destinationPortal.transform.position;
            travelCost ++;
            // 4b. NOTIFICA IL GESTORE DELLA DIFFICOLT�
            if (DifficultyManager.Instance != null)
            {
                DifficultyManager.Instance.RegisterPortalUse();
            }
        }
        else
        {
            Debug.LogWarning("Player con tag 'Player' non trovato in scena.");
        }
    }

    public bool isDestinationPortal()
    {
        return portalType == PortalType.Out;
    }

    public int getTravelCost()
    {
        return travelCost;
    }

    public void setTravelCost(int newCost)
    {
        travelCost = newCost;
    }
}
