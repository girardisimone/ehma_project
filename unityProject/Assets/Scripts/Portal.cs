using UnityEngine;
using static PortalManager;
public class Portal : MonoBehaviour
{
    public enum PortalType { StartOnly, EndOnly, Both }
    public enum ActivationMode { Automatic, OnButtonPress }
    private PortalManager portalManager;
    [Header("🔷 Configurazione Base")]
    [Tooltip("Il tipo di portale: solo partenza, solo arrivo o entrambi.")]
    public PortalType portalType = PortalType.Both;

    [Tooltip("Se falso, il portale sarà invisibile/disattivo nella scena.")]
    public bool isActive = true;

    [Tooltip("Il costo in gemme richiesto per usare questo portale.")]
    public int travelCost = 5;

    [Tooltip("Come viene attivato il portale.")]
    public ActivationMode activationMode = ActivationMode.Automatic;

    [Header("🔗 Collegamenti")]
    [Tooltip("Il portale di destinazione.")]
    public Portal destinationPortal;

    private bool isPlayerInside = false;
    private Transform player;

    private void Start()
    {
        // Imposta visibilità
        gameObject.SetActive(isActive);

        // Registrazione nel manager
        if (PortalManager.Instance != null)
        {
            PortalManager.Instance.RegisterPortal(this);
            portalManager = PortalManager.Instance;
        }
        else
            Debug.LogWarning($"Nessun PortalManager trovato per {name}");
    }

    private void Update()
    {
        // Se il portale richiede un pulsante e il giocatore è dentro
        if (activationMode == ActivationMode.OnButtonPress && isPlayerInside && isActive)
        {
            if (Input.GetKeyDown(KeyCode.X))
            {
                TryTeleport();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!isActive || portalType == PortalType.EndOnly) return;

        if (other.CompareTag("Player"))
        {
            isPlayerInside = true;
            player = other.transform;

            if (activationMode == ActivationMode.Automatic)
                TryTeleport();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInside = false;
            player = null;
        }
    }

    public bool isDestinationPortal()
    {
        return portalType == PortalType.EndOnly || portalType == PortalType.Both;
    }

    /// <summary>
    /// Tenta il teletrasporto del giocatore, verificando gemme e portale di destinazione.
    /// </summary>
    private void TryTeleport()
    {
        if (portalManager.isRandomConnections())
        {
            destinationPortal = portalManager.getRandomGrid().getDestinationPortal();
        }
        
        if (destinationPortal == null)
        {
            Debug.LogWarning($"{name} non ha un portale di destinazione.");
            return;
        }

        if (ScoreManager.GemCount < travelCost)
        {
            Debug.Log($"Gemme insufficienti per usare {name}. Servono {travelCost}, hai {ScoreManager.GemCount}.");
            return;
        }

        // Pagamento
        ScoreManager.GemCount -= travelCost;
        ScoreManager.Instance?.UpdateScoreText(ScoreManager.GemCount);

        // Teletrasporto
        player.position = destinationPortal.transform.position;

        Debug.Log($"{name}: teletrasporto completato. Costo {travelCost} gemme.");
        
        // aumento del costo del portale dopo ogni utilizzo
        // Da capire con che logica
        travelCost++;
    }

    // --- Metodi pubblici per gestione runtime dal Manager ---
    public void SetActive(bool value)
    {
        isActive = value;
        gameObject.SetActive(value);
    }

    public void SetTravelCost(int cost)
    {
        travelCost = cost;
    }

    public void SetActivationMode(ActivationMode mode)
    {
        activationMode = mode;
    }

    public void SetDestination(Portal dest)
    {
        destinationPortal = dest;
    }

    public void SetType(PortalType type)
    {
        portalType = type;
    }

    
}
