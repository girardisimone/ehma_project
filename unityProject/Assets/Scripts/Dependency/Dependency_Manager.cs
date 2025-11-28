using UnityEngine;
using System;
using System.Collections;

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

    [Header("Durate Malus (Secondi)")]
    public float durationDrugs = 15f;
    public float durationGambling = 8f;
    public float durationInternet = 5f;

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

        switch (charIndex)
        {
            case 0: break; // Sano
            case 1: AddDependency(DependencyType.Drugs); break;
            case 2: AddDependency(DependencyType.Gambling); break;
            case 3: AddDependency(DependencyType.Internet); break;
            
        }
    }

    // Chiamato dal Portale
    public void ApplyMovementMalus(GameObject player)
    {
        StartCoroutine(TemporaryMalusCoroutine(player));
    }

    IEnumerator TemporaryMalusCoroutine(GameObject player)
    {
        NewPlayerMovement playerMovement = player.GetComponent<NewPlayerMovement>();
        if (playerMovement == null) yield break;

        // --- NUOVO: CONTROLLO IMMUNITÀ ---
        if (playerMovement.isImmuneToMalus)
        {
            Debug.Log($"[DependencyManager] Il giocatore è IMMUNE. Nessun malus applicato.");
            yield break; // Esce dalla coroutine immediatamente
        }

        IMovementStrategy strategyToApply = new NormalMovementStrategy();
        float baseDuration = 0f;
        
        // --- LOGICA DI SCELTA (Invariata) ---
        bool malusSelected = false; // Flag per capire se abbiamo scelto qualcosa

        if (HasDependency(DependencyType.Internet))
        {
            baseDuration = durationInternet;
            malusSelected = true;

            int rand = UnityEngine.Random.Range(0, 2);
            if (rand == 0) strategyToApply = new InternetAddictStrategy();
            else strategyToApply = new InternetPacketLossStrategy();

            if (UnityEngine.Random.value < 1.1f && DifficultyManager.Instance != null)
                DifficultyManager.Instance.ForceGlitch(baseDuration * playerMovement.malusDurationMultiplier);
        }
        else if (HasDependency(DependencyType.Drugs))
        {
            baseDuration = durationDrugs;
            malusSelected = true;

            int rand = UnityEngine.Random.Range(0, 3);
            if (rand == 0) strategyToApply = new DruggedStrategy();
            else if (rand == 1) strategyToApply = new DrunkStrategy();
            else strategyToApply = new DruggedSpinningStrategy();

            if (UnityEngine.Random.value < 0.8f && DifficultyManager.Instance != null)
                DifficultyManager.Instance.ForceDarkness(baseDuration * playerMovement.malusDurationMultiplier);
        }
        else if (HasDependency(DependencyType.Gambling))
        {
            baseDuration = durationGambling;
            malusSelected = true;

            int rand = UnityEngine.Random.Range(0, 2);
            if (rand == 0) strategyToApply = new GamblerStrategy();
            else strategyToApply = new GamblerRouletteStrategy();

            if (UnityEngine.Random.value < 0.5f && DifficultyManager.Instance != null)
                DifficultyManager.Instance.ForceDarkness(baseDuration * playerMovement.malusDurationMultiplier);
        }

        if (!malusSelected) yield break; // Se non ha dipendenze, usciamo

        // --- NUOVO: CALCOLO DURATA EFFETTIVA ---
        // Moltiplichiamo la durata base per il moltiplicatore del player
        float effectiveDuration = baseDuration * playerMovement.malusDurationMultiplier;

        Debug.Log($"Malus attivato per {effectiveDuration} secondi (Base: {baseDuration} * Molt: {playerMovement.malusDurationMultiplier})");

        // --- APPLICAZIONE ---
        playerMovement.SetStrategy(strategyToApply);

        yield return new WaitForSeconds(effectiveDuration);

        // --- RIPRISTINO ---
        playerMovement.SetStrategy(new NormalMovementStrategy());
    }

    public void AddDependency(DependencyType type) { currentDependencies |= type; }
    public void RemoveDependency(DependencyType type) { currentDependencies &= ~type; }
    public bool HasDependency(DependencyType type) { return (currentDependencies & type) != 0; }
}