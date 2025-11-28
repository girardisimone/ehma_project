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

    // --- NUOVO: VARIABILE PER ACCUMULARE IL COSTO ---
    [HideInInspector]
    public int gamblingDebt = 0; // Di quanto aumenta il costo? (0, 5, 10, 15...)

    private int paymentCycleIndex = 0;

    private void Awake()
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

        // RESETTA IL DEBITO ALL'INIZIO DELLA PARTITA
        gamblingDebt = 0;

        switch (charIndex)
        {
            case 0: break;
            case 1: AddDependency(DependencyType.Drugs); break;
            case 2: AddDependency(DependencyType.Gambling); break;
            case 3: AddDependency(DependencyType.Internet); break;
        }
    }

    // --- METODO PER AUMENTARE IL DEBITO (Chiamato dopo aver pagato) ---
    public void IncreaseGamblingDebt(int amount)
    {
        gamblingDebt += amount;
        Debug.Log($"[GAMBLING] Il debito è salito! Prossimo extra costo: +{gamblingDebt}");
    }

    // ... (Il resto del codice GetNextPaymentDependency, ApplyMovementMalus, ecc. rimane UGUALE) ...
    // ... COPIA QUI SOTTO TUTTO IL RESTO DELLO SCRIPT CHE AVEVI PRIMA ...

    public DependencyType GetNextPaymentDependency()
    {
        List<DependencyType> activeList = new List<DependencyType>();
        foreach (DependencyType type in Enum.GetValues(typeof(DependencyType)))
        {
            if (type == DependencyType.None) continue;
            if (HasDependency(type)) activeList.Add(type);
        }
        if (activeList.Count == 0) return DependencyType.None;
        return activeList[paymentCycleIndex % activeList.Count];
    }

    public void AdvancePaymentCycle() { paymentCycleIndex++; }

    public void ApplyMovementMalus(GameObject player) { StartCoroutine(TemporaryMalusCoroutine(player)); }

    IEnumerator TemporaryMalusCoroutine(GameObject player)
    {
        // ... (Copia il contenuto della Coroutine dallo script precedente) ...
        // ... (Non cambia nulla qui dentro) ...

        NewPlayerMovement playerMovement = player.GetComponent<NewPlayerMovement>();
        if (playerMovement == null) yield break;

        IMovementStrategy strategyToApply = new NormalMovementStrategy();
        float currentDuration = 0f;

        if (HasDependency(DependencyType.Internet))
        {
            currentDuration = durationInternet;
            int rand = UnityEngine.Random.Range(0, 2);
            if (rand == 0) strategyToApply = new InternetAddictStrategy();
            else strategyToApply = new InternetPacketLossStrategy();

            if (UnityEngine.Random.value < 0.3f && DifficultyManager.Instance != null)
                DifficultyManager.Instance.ForceGlitch(currentDuration);
        }
        else if (HasDependency(DependencyType.Drugs))
        {
            currentDuration = durationDrugs;
            int rand = UnityEngine.Random.Range(0, 3);
            if (rand == 0) strategyToApply = new DruggedStrategy();
            else if (rand == 1) strategyToApply = new DrunkStrategy();
            else strategyToApply = new DruggedSpinningStrategy();

            if (UnityEngine.Random.value < 0.8f && DifficultyManager.Instance != null)
                DifficultyManager.Instance.ForceDarkness(currentDuration);
        }
        else if (HasDependency(DependencyType.Gambling))
        {
            currentDuration = durationGambling;
            int rand = UnityEngine.Random.Range(0, 2);
            if (rand == 0) strategyToApply = new GamblerStrategy();
            else strategyToApply = new GamblerRouletteStrategy();

            if (UnityEngine.Random.value < 0.5f && DifficultyManager.Instance != null)
                DifficultyManager.Instance.ForceDarkness(currentDuration);
        }
        else
        {
            yield break;
        }

        playerMovement.SetStrategy(strategyToApply);
        yield return new WaitForSeconds(currentDuration);
        playerMovement.SetStrategy(new NormalMovementStrategy());
    }

    public void AddDependency(DependencyType type) { currentDependencies |= type; }
    public void RemoveDependency(DependencyType type) { currentDependencies &= ~type; }
    public bool HasDependency(DependencyType type) { return (currentDependencies & type) != 0; }
}