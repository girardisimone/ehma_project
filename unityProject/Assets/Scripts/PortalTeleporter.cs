using UnityEngine;
using TMPro;

[RequireComponent(typeof(BoxCollider2D))]
public class PortalTeleporter : MonoBehaviour
{
    [Header("Impostazioni Portale")]
    public PortalTeleporter destinationPortal;
    public enum PortalType { In, Out }
    public PortalType portalType = PortalType.In;

    [Header("Costi Base")]
    public int costGems = 3;
    public int costHealth = 2;
    public float costTime = 30f;

    [Header("Incremento Gambling")]
    public int gamblingCostIncrease = 3;

    // --- NUOVO: Variabile per il suono ---
    [Header("Audio")]
    public AudioClip teleportSound;
    // -------------------------------------

    private bool isPlayerInside = false;
    private PortalManager portalManager;
    private NewPlayerMovement playerMovementScript;

    private DependencyType assignedPaymentType = DependencyType.None;
    private int finalCalculatedGemCost = 0;

    private void Start()
    {
        if (portalType == PortalType.Out)
        {
            SpriteRenderer sr = GetComponent<SpriteRenderer>();
            if (sr != null) sr.enabled = false;
            foreach (var child in GetComponentsInChildren<SpriteRenderer>()) child.enabled = false;
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

        isPlayerInside = true;
        playerMovementScript = other.GetComponent<NewPlayerMovement>();
        if (playerMovementScript != null) playerMovementScript.enabled = false;

        string costString = "";
        bool canAfford = false;

        assignedPaymentType = DependencyType.None;
        if (DependencyManager.Instance != null)
        {
            assignedPaymentType = DependencyManager.Instance.GetNextPaymentDependency();
        }

        switch (assignedPaymentType)
        {
            case DependencyType.Internet:
                canAfford = true;
                costString = $"Cost:+{costTime}s";
                break;

            case DependencyType.Drugs:
                if (HealthManager.Instance != null)
                {
                    canAfford = HealthManager.Instance.currentHealth > costHealth;
                    string cuori = (costHealth / 4f).ToString("0.#");
                    costString = $"Cost:-{cuori} Health";
                }
                break;

            case DependencyType.Gambling:
                int currentDebt = DependencyManager.Instance != null ? DependencyManager.Instance.gamblingDebt : 0;
                finalCalculatedGemCost = costGems + currentDebt;

                canAfford = ScoreManager.GemCount >= finalCalculatedGemCost;
                costString = $"Cost: {finalCalculatedGemCost} Gems";
                break;

            default:
                finalCalculatedGemCost = costGems;
                canAfford = ScoreManager.GemCount >= finalCalculatedGemCost;
                costString = $"Costo: {finalCalculatedGemCost} Gemme";
                break;
        }

        if (PortalPopupController.Instance != null)
        {
            PortalPopupController.Instance.Show(this, canAfford, costString);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (portalType == PortalType.Out) return;

        if (PortalPopupController.Instance != null)
            PortalPopupController.Instance.HideIfCurrent(this);

        if (playerMovementScript != null) playerMovementScript.enabled = true;
        isPlayerInside = false;
    }

    public void TeleportPlayerAndPay()
    {
        if (!isPlayerInside) return;

        // --- NUOVO: SUONO TELETRASPORTO ---
        if (teleportSound != null)
        {
            // Riproduce il suono nel punto dove si trova il portale
            AudioSource.PlayClipAtPoint(teleportSound, transform.position);
        }
        // ----------------------------------

        switch (assignedPaymentType)
        {
            case DependencyType.Internet:
                TimerManager timer = FindAnyObjectByType<TimerManager>();
                if (timer != null) timer.AddTimePenalty(costTime);
                break;

            case DependencyType.Drugs:
                if (HealthManager.Instance != null) HealthManager.Instance.TakeDamage(costHealth);
                break;

            case DependencyType.Gambling:
                ScoreManager.GemCount -= finalCalculatedGemCost;
                if (ScoreManager.Instance != null) ScoreManager.Instance.UpdateScoreText(ScoreManager.GemCount);

                if (DependencyManager.Instance != null)
                {
                    DependencyManager.Instance.IncreaseGamblingDebt(gamblingCostIncrease);
                }
                break;

            default:
                ScoreManager.GemCount -= costGems;
                if (ScoreManager.Instance != null) ScoreManager.Instance.UpdateScoreText(ScoreManager.GemCount);
                break;
        }

        // --- TUA LOGICA CANE MANTENUTA ---
        DogCompanion cane = FindAnyObjectByType<DogCompanion>();
        if (cane != null)
        {
            cane.RestaQui();
        }
        // ---------------------------------

        if (DependencyManager.Instance != null)
        {
            DependencyManager.Instance.AdvancePaymentCycle();
            // --- TUA LOGICA MALUS MANTENUTA (con parametro extra) ---
            DependencyManager.Instance.ApplyMovementMalus(playerMovementScript.gameObject, assignedPaymentType);
        }

        if (DifficultyManager.Instance != null)
        {
            DifficultyManager.Instance.RegisterPortalUse();
        }

        if (portalManager != null && portalManager.isRandomConnections())
        {
            var grid = portalManager.getRandomGrid();
            if (grid != null) destinationPortal = grid.getDestinationPortal();
        }

        if (destinationPortal != null && playerMovementScript != null)
        {
            playerMovementScript.transform.position = destinationPortal.transform.position;
            Destroy(destinationPortal.gameObject);
        }

        if (playerMovementScript != null) playerMovementScript.enabled = true;
        isPlayerInside = false;
    }

    public bool isDestinationPortal() { return portalType == PortalType.Out; }
    public void setTravelCost(int newCost) { costGems = newCost; }
}