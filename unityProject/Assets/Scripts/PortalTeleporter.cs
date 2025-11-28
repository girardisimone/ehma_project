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
    public int costHealth = 1;
    public float costTime = 30f;

    [Header("Incremento Gambling")]
    public int gamblingCostIncrease = 3; // Di quanto aumenta ogni volta?

    private bool isPlayerInside = false;
    private PortalManager portalManager;
    private NewPlayerMovement playerMovementScript;

    // Variabili per memorizzare lo stato attuale di questo passaggio
    private DependencyType assignedPaymentType = DependencyType.None;
    private int finalCalculatedGemCost = 0; // Per ricordare quanto costa QUESTO passaggio specifico

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

        // 1. CHIEDIAMO IL TIPO DI PAGAMENTO
        assignedPaymentType = DependencyType.None;
        if (DependencyManager.Instance != null)
        {
            assignedPaymentType = DependencyManager.Instance.GetNextPaymentDependency();
        }

        // 2. CALCOLO COSTI
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
                // --- LOGICA INCREMENTALE ---
                // Il costo è: Base (3) + Debito Accumulato (0, 5, 10...)
                int currentDebt = DependencyManager.Instance != null ? DependencyManager.Instance.gamblingDebt : 0;
                finalCalculatedGemCost = costGems + currentDebt;

                canAfford = ScoreManager.GemCount >= finalCalculatedGemCost;
                costString = $"Cost: {finalCalculatedGemCost} Gems";
                break;

            default:
                // Nessuna dipendenza: Costo base
                finalCalculatedGemCost = costGems;
                canAfford = ScoreManager.GemCount >= finalCalculatedGemCost;
                costString = $"Costo: {finalCalculatedGemCost} Gemme";
                break;
        }

        // 3. MOSTRA POPUP
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

        // 4. ESEGUI PAGAMENTO
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
                // Paga il costo calcolato (Base + Debito)
                ScoreManager.GemCount -= finalCalculatedGemCost;
                if (ScoreManager.Instance != null) ScoreManager.Instance.UpdateScoreText(ScoreManager.GemCount);

                // --- AUMENTA IL DEBITO PER LA PROSSIMA VOLTA! ---
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

        // 5. AVANZA CICLO E ATTIVA MALUS
        if (DependencyManager.Instance != null)
        {
            DependencyManager.Instance.AdvancePaymentCycle();
            DependencyManager.Instance.ApplyMovementMalus(playerMovementScript.gameObject);
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