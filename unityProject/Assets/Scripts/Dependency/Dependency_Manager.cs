using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

[Flags]
public enum DependencyType
{
    None = 0,
    Drugs = 1,
    Gambling = 2,
    Internet = 4
}

public class DependencyManager : MonoBehaviour
{
    public static DependencyManager Instance;

    public DependencyType currentDependencies;

    [Header("Configurazione Durate Malus")]
    public float durationDrugs = 15f;
    public float durationGambling = 8f;
    public float durationInternet = 5f;

    [HideInInspector]
    public int gamblingDebt = 0;

    // --- NUOVI CONTATORI PER CICLICITÀ ---
    private int paymentCycleIndex = 0; // Per alternare il tipo di pagamento
    private int cycleInternet = 0;     // Per alternare i malus Internet
    private int cycleDrugs = 0;        // Per alternare i malus Droghe
    private int cycleGambling = 0;     // Per alternare i malus Gambling

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        InitializeCharacterDependency();
    }

    void InitializeCharacterDependency()
    {
        int charIndex = PlayerPrefs.GetInt("SelectedCharacter", 0);
        currentDependencies = DependencyType.None;
        gamblingDebt = 0;

        // Resetta i cicli all'inizio
        paymentCycleIndex = 0;
        cycleInternet = 0;
        cycleDrugs = 0;
        cycleGambling = 0;

        switch (charIndex)
        {
            case 0: break;
            case 1: AddDependency(DependencyType.Drugs); break;
            case 2: AddDependency(DependencyType.Gambling); break;
            case 3: AddDependency(DependencyType.Internet); break;
        }
    }

    public void AddDependency(DependencyType type)
    {
        currentDependencies |= type;
        Debug.Log("Nuova dipendenza acquisita: " + type);
    }

    // --- METODO CHIAMATO DAL PORTALE (ORA CICLICO) ---
    public DependencyType GetNextPaymentDependency()
    {
        List<DependencyType> activeList = new List<DependencyType>();

        foreach (DependencyType type in Enum.GetValues(typeof(DependencyType)))
        {
            if (type == DependencyType.None) continue;
            if (HasDependency(type)) activeList.Add(type);
        }

        if (activeList.Count == 0) return DependencyType.None;

        // MODIFICA: Uso il modulo (%) invece del Random
        return activeList[paymentCycleIndex % activeList.Count];
    }

    public void IncreaseGamblingDebt(int amount)
    {
        gamblingDebt += amount;
        Debug.Log($"[GAMBLING] Il debito è salito! Prossimo extra costo: +{gamblingDebt}");
    }

    // MODIFICA: Ora incrementa l'indice per il prossimo pagamento
    public void AdvancePaymentCycle()
    {
        paymentCycleIndex++;
    }

    public bool HasDependency(DependencyType type) { return (currentDependencies & type) != 0; }

    public void ApplyMovementMalus(GameObject player, DependencyType typeToApply)
    {
        StartCoroutine(TemporaryMalusCoroutine(player, typeToApply));
    }

    IEnumerator TemporaryMalusCoroutine(GameObject player, DependencyType type)
    {
        NewPlayerMovement playerMovement = player.GetComponent<NewPlayerMovement>();
        if (playerMovement == null) yield break;

        IMovementStrategy strategyToApply = new NormalMovementStrategy();
        float currentDuration = 0f;

        switch (type)
        {
            case DependencyType.Internet:
                currentDuration = durationInternet;

                // MODIFICA CICLICA (0, 1, 0, 1...)
                int indexInternet = cycleInternet % 2;
                strategyToApply = (indexInternet == 0) ? (IMovementStrategy)new InternetAddictStrategy() : new InternetPacketLossStrategy();

                cycleInternet++; // Avanza il ciclo per la prossima volta

                if (DifficultyManager.Instance != null)
                {
                    DifficultyManager.Instance.ForceGlitch(currentDuration);
                }
                break;

            case DependencyType.Drugs:
                currentDuration = durationDrugs;

                // MODIFICA CICLICA (0, 1, 2, 0, 1, 2...)
                int indexDrugs = cycleDrugs % 3;
                if (indexDrugs == 0) strategyToApply = new DruggedStrategy();
                else if (indexDrugs == 1) strategyToApply = new DrunkStrategy();
                else strategyToApply = new DruggedSpinningStrategy();

                cycleDrugs++; // Avanza il ciclo

                if (UnityEngine.Random.value < 0.8f && DifficultyManager.Instance != null)
                    DifficultyManager.Instance.ForceDarkness(currentDuration);
                break;

            case DependencyType.Gambling:
                currentDuration = durationGambling;

                // MODIFICA CICLICA (0, 1, 0, 1...)
                int indexGambling = cycleGambling % 2;
                strategyToApply = (indexGambling == 0) ? (IMovementStrategy)new GamblerStrategy() : new GamblerRouletteStrategy();

                cycleGambling++; // Avanza il ciclo

                if (UnityEngine.Random.value < 0.5f && DifficultyManager.Instance != null)
                    DifficultyManager.Instance.ForceDarkness(currentDuration);
                break;

            default:
                yield break;
        }

        playerMovement.SetStrategy(strategyToApply);
        yield return new WaitForSeconds(currentDuration);
        playerMovement.SetStrategy(new NormalMovementStrategy());
    }
}