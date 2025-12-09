using UnityEngine;
using System.Collections;
using TMPro;

[RequireComponent(typeof(AudioSource))]
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

    [Header("Audio")]
    public AudioClip teleportSound;      // Suono Partenza
    public AudioClip arrivalSound;       // Suono Arrivo
    [Range(0f, 1f)]
    public float volumeTeletrasporto = 1f;

    // --- NUOVO: VARIABILE PER IL TEMPO ---
    [Header("Tempistiche")]
    public float ritardoTeletrasporto = 3f; // Default 3 secondi, modificabile nell'Inspector
    // -------------------------------------

    private AudioSource audioSource;
    private bool isPlayerInside = false;
    private PortalManager portalManager;
    private NewPlayerMovement playerMovementScript;

    private DependencyType assignedPaymentType = DependencyType.None;
    private int finalCalculatedGemCost = 0;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0f;

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
            assignedPaymentType = DependencyManager.Instance.GetNextPaymentDependency();

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
            PortalPopupController.Instance.Show(this, canAfford, costString);
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
        StartCoroutine(SequenceTeleport());
    }

    IEnumerator SequenceTeleport()
    {
        // 1. SUONA PARTENZA
        if (teleportSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(teleportSound, volumeTeletrasporto);
        }

        // 2. ASPETTA IL TEMPO IMPOSTATO (Ora usa la variabile)
        yield return new WaitForSeconds(ritardoTeletrasporto);

        // --- Logica Pagamenti ---
        PayCost();

        DogCompanion cane = FindAnyObjectByType<DogCompanion>();
        if (cane != null) cane.RestaQui();

        if (DependencyManager.Instance != null)
        {
            DependencyManager.Instance.AdvancePaymentCycle();
            DependencyManager.Instance.ApplyMovementMalus(playerMovementScript.gameObject, assignedPaymentType);
        }

        if (DifficultyManager.Instance != null) DifficultyManager.Instance.RegisterPortalUse();

        if (portalManager != null && portalManager.isRandomConnections())
        {
            var grid = portalManager.getRandomGrid();
            if (grid != null) destinationPortal = grid.getDestinationPortal();
        }

        if (destinationPortal == null)
        {
            Debug.LogWarning($"Il destinational portal e' null");
        }
        // --- 3. TELETRASPORTO ---
        if (destinationPortal != null && playerMovementScript != null)
        {
            playerMovementScript.transform.position = destinationPortal.transform.position;

            // 4. SUONA ARRIVO
            if (arrivalSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(arrivalSound, volumeTeletrasporto);
            }
            
            // il samurai deve essere avvisato se il giocatore usa il portale
            if (samurai_interaction.Instance != null)
            {
                samurai_interaction.Instance.GiocatoreHaUsatoPortale();
            }
            //Destroy(destinationPortal.gameObject, 0.1f);
        }

        if (playerMovementScript != null) playerMovementScript.enabled = true;
        isPlayerInside = false;
    }

    void PayCost()
    {
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
                if (DependencyManager.Instance != null) DependencyManager.Instance.IncreaseGamblingDebt(gamblingCostIncrease);
                break;
            default:
                ScoreManager.GemCount -= costGems;
                if (ScoreManager.Instance != null) ScoreManager.Instance.UpdateScoreText(ScoreManager.GemCount);
                break;
        }
    }

    public bool isDestinationPortal() { return portalType == PortalType.Out; }
    public void setTravelCost(int newCost) { costGems = newCost; }
}