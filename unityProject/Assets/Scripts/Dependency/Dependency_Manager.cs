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

    // Questa variabile contiene TUTTE le dipendenze attive insieme (grazie ai Flags)
    public DependencyType currentDependencies;

    [Header("Configurazione Durate Malus")]
    public float durationDrugs = 15f;
    public float durationGambling = 8f;
    public float durationInternet = 5f;

    [HideInInspector]
    public int gamblingDebt = 0;

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

        switch (charIndex)
        {
            case 0: break; // Topo (O default)
            case 1: AddDependency(DependencyType.Drugs); break;
            case 2: AddDependency(DependencyType.Gambling); break;
            case 3: AddDependency(DependencyType.Internet); break;
        }
    }

    // --- METODO CHIAMATO DALL'NPC CATTIVO ---
    public void AddDependency(DependencyType type)
    {
        // L'operatore |= aggiunge il flag senza cancellare gli altri
        currentDependencies |= type;
        Debug.Log("Nuova dipendenza acquisita: " + type);
    }

    // --- METODO CHIAMATO DAL PORTALE (LOGICA CASUALE) ---
    public DependencyType GetNextPaymentDependency()
    {
        // 1. Creiamo una lista temporanea di tutte le dipendenze attive
        List<DependencyType> activeList = new List<DependencyType>();

        foreach (DependencyType type in Enum.GetValues(typeof(DependencyType)))
        {
            if (type == DependencyType.None) continue;

            // Se il giocatore HA questa dipendenza, aggiungila alla lista possibile
            if (HasDependency(type)) activeList.Add(type);
        }

        // 2. Se non ne ha nessuna, ritorna None
        if (activeList.Count == 0) return DependencyType.None;

        // 3. PESCA CASUALE (Random) dalla lista
        int randomIndex = UnityEngine.Random.Range(0, activeList.Count);
        return activeList[randomIndex];
    }

    // Aumenta il debito Gambling
    public void IncreaseGamblingDebt(int amount)
    {
        gamblingDebt += amount;
        Debug.Log($"[GAMBLING] Il debito è salito! Prossimo extra costo: +{gamblingDebt}");
    }

    // Questa serviva per la sequenza, ora col random serve meno ma la lasciamo per compatibilità
    public void AdvancePaymentCycle() { }

    // Helper per verificare se hai una dipendenza
    public bool HasDependency(DependencyType type) { return (currentDependencies & type) != 0; }

    // --- GESTIONE MALUS (INVARIATA) ---
    // --- MODIFICA QUI: Ora chiediamo ANCHE "typeToApply" ---
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

        // --- QUI USIAMO LO SWITCH SUL TIPO PASSATO, NON SUI FLAGS ---
        switch (type)
        {
            case DependencyType.Internet:
                // Pagato con TEMPO -> Malus Glitch/Packet Loss
                currentDuration = durationInternet;
                int randInt = UnityEngine.Random.Range(0, 2);

                // Sceglie la strategia di movimento (Lag o Movimento a scatti)
                strategyToApply = (randInt == 0) ? (IMovementStrategy)new InternetAddictStrategy() : new InternetPacketLossStrategy();

                // --- MODIFICA QUI ---
                // Abbiamo tolto "UnityEngine.Random.value < 0.3f"
                // Ora controlliamo solo se il DifficultyManager esiste
                if (DifficultyManager.Instance != null)
                {
                    DifficultyManager.Instance.ForceGlitch(currentDuration);
                }
                break;

            case DependencyType.Drugs:
                // Pagato con VITA -> Malus Ubriachezza/Drogato
                currentDuration = durationDrugs;
                int randDrug = UnityEngine.Random.Range(0, 3);
                if (randDrug == 0) strategyToApply = new DruggedStrategy();
                else if (randDrug == 1) strategyToApply = new DrunkStrategy();
                else strategyToApply = new DruggedSpinningStrategy();

                if (UnityEngine.Random.value < 0.8f && DifficultyManager.Instance != null)
                    DifficultyManager.Instance.ForceDarkness(currentDuration);
                break;

            case DependencyType.Gambling:
                // Pagato con GEMME -> Malus Oscurità/Roulette
                currentDuration = durationGambling;
                int randGamb = UnityEngine.Random.Range(0, 2);
                strategyToApply = (randGamb == 0) ? (IMovementStrategy)new GamblerStrategy() : new GamblerRouletteStrategy();

                if (UnityEngine.Random.value < 0.5f && DifficultyManager.Instance != null)
                    DifficultyManager.Instance.ForceDarkness(currentDuration);
                break;

            // Se paga "senza dipendenza" (es. costo base), nessun malus
            default:
                yield break;
        }

        // Applica strategia
        playerMovement.SetStrategy(strategyToApply);

        // Aspetta
        yield return new WaitForSeconds(currentDuration);

        // Ripristina
        playerMovement.SetStrategy(new NormalMovementStrategy());
    }
}
        