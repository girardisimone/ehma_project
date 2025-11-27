using UnityEngine;
using TMPro;

[RequireComponent(typeof(BoxCollider2D))]
public class PortalTeleporter : MonoBehaviour
{
    [Header("Impostazioni Portale")]
    public PortalTeleporter destinationPortal;
    public int travelCost = 3;
    public enum PortalType { In, Out }
    public PortalType portalType = PortalType.In;

    // Questa variabile causava il warning: ora la useremo!
    private bool isPlayerInside = false;

    private PortalManager portalManager;
    private NewPlayerMovement playerMovementScript;

    private void Start()
    {
        if (portalType == PortalType.Out)
        {
            SpriteRenderer sr = GetComponent<SpriteRenderer>();
            if (sr != null) sr.enabled = false;

            foreach (var child in GetComponentsInChildren<SpriteRenderer>())
                child.enabled = false;
        }

        if (PortalManager.Instance != null)
        {
            PortalManager.Instance.RegisterPortal(this);
            portalManager = PortalManager.Instance;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (portalType == PortalType.Out) return;

        isPlayerInside = true; // Segniamo che il player è dentro
        playerMovementScript = other.GetComponent<NewPlayerMovement>();

        if (playerMovementScript != null) playerMovementScript.enabled = false;

        bool canUsePortal = ScoreManager.GemCount >= travelCost;

        if (PortalPopupController.Instance != null)
        {
            PortalPopupController.Instance.Show(this, canUsePortal, travelCost);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (portalType == PortalType.Out) return;

        if (PortalPopupController.Instance != null)
        {
            PortalPopupController.Instance.HideIfCurrent(this);
        }

        if (playerMovementScript != null) playerMovementScript.enabled = true;

        isPlayerInside = false; // Segniamo che il player è uscito
    }

    // Chiamato dal tasto "SÌ" del Popup
    public void TeleportPlayerAndPay()
    {
        // --- CORREZIONE WARNING ---
        // Controlliamo se il giocatore è ancora fisicamente nel portale.
        // Se si è allontanato (es. spinto via o bug), annulliamo tutto.
        if (!isPlayerInside)
        {
            Debug.Log("Azione annullata: il giocatore non è più nel portale.");
            return;
        }
        // --------------------------

        // 1. Paga il costo
        ScoreManager.GemCount -= travelCost;

        if (ScoreManager.Instance != null)
            ScoreManager.Instance.UpdateScoreText(ScoreManager.GemCount);

        // 2. Avvisa il Difficulty Manager (Effetti, Malus Movimento, ecc.)
        if (DifficultyManager.Instance != null)
        {
            // NOTA: Questo ora chiamerà anche ApplyMovementMalus se lo hai configurato lì
            DifficultyManager.Instance.RegisterPortalUse();

            // Se vuoi attivare il malus movimento SPECIFICO da qui, usa DependencyManager:
            if (DependencyManager.Instance != null && playerMovementScript != null)
            {
                DependencyManager.Instance.ApplyMovementMalus(playerMovementScript.gameObject);
            }
        }

        // 3. Calcola Destinazione
        if (portalManager != null && portalManager.isRandomConnections())
        {
            var grid = portalManager.getRandomGrid();
            if (grid != null) destinationPortal = grid.getDestinationPortal();
        }

        // 4. Teletrasporto
        if (destinationPortal != null && playerMovementScript != null)
        {
            playerMovementScript.transform.position = destinationPortal.transform.position;
            Destroy(destinationPortal.gameObject);
        }

        // 5. Riabilita Movimento
        if (playerMovementScript != null) playerMovementScript.enabled = true;

        isPlayerInside = false;
    }

    public bool isDestinationPortal() { return portalType == PortalType.Out; }
    public void setTravelCost(int newCost) { travelCost = newCost; }
}