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

        IMovementStrategy strategyToApply = new NormalMovementStrategy();
        float currentDuration = 0f;

        // --- LOGICA DI SCELTA ---

        if (HasDependency(DependencyType.Internet))
        {
            currentDuration = durationInternet;

            // 1. Movimento (Lag)
            int rand = UnityEngine.Random.Range(0, 2);
            if (rand == 0) strategyToApply = new InternetAddictStrategy();
            else strategyToApply = new InternetPacketLossStrategy();

            // 2. Glitch Visivo (30% probabilità)
            if (UnityEngine.Random.value < 1.1f && DifficultyManager.Instance != null)
            {
                DifficultyManager.Instance.ForceGlitch(currentDuration);
            }

            Debug.Log("Malus Internet attivato");
        }
        else if (HasDependency(DependencyType.Drugs))
        {
            currentDuration = durationDrugs;

            // 1. Movimento (Confusione)
            int rand = UnityEngine.Random.Range(0, 3);
            if (rand == 0) strategyToApply = new DruggedStrategy();
            else if (rand == 1) strategyToApply = new DrunkStrategy();
            else strategyToApply = new DruggedSpinningStrategy();

            // 2. Buio (80% probabilità)
            if (UnityEngine.Random.value < 0.8f && DifficultyManager.Instance != null)
            {
                DifficultyManager.Instance.ForceDarkness(currentDuration);
            }

            Debug.Log("Malus Droghe attivato");
        }
        else if (HasDependency(DependencyType.Gambling))
        {
            currentDuration = durationGambling;

            // 1. Movimento (Random)
            int rand = UnityEngine.Random.Range(0, 2);
            if (rand == 0) strategyToApply = new GamblerStrategy();
            else strategyToApply = new GamblerRouletteStrategy();

            // 2. Buio (50% probabilità)
            if (UnityEngine.Random.value < 0.5f && DifficultyManager.Instance != null)
            {
                DifficultyManager.Instance.ForceDarkness(currentDuration);
            }

            Debug.Log("Malus Gambling attivato");
        }
        else
        {
            yield break; // Nessuna dipendenza
        }

        // --- APPLICAZIONE ---
        playerMovement.SetStrategy(strategyToApply);

        yield return new WaitForSeconds(currentDuration);

        // --- RIPRISTINO ---
        playerMovement.SetStrategy(new NormalMovementStrategy());
    }

    public void AddDependency(DependencyType type) { currentDependencies |= type; }
    public void RemoveDependency(DependencyType type) { currentDependencies &= ~type; }
    public bool HasDependency(DependencyType type) { return (currentDependencies & type) != 0; }
}