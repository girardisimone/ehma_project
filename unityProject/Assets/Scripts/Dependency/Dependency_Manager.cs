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

    [Header("Configurazione Durate Malus")]
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
        Debug.Log($"[DEBUG] Indice Personaggio Caricato: {charIndex}"); // <--- CONTROLLA QUESTO

        currentDependencies = DependencyType.None;

        // VERIFICA CHE QUESTI NUMERI SIANO GLI STESSI DEL TUO MENU
        switch (charIndex)
        {
            case 0:
                Debug.Log("[DEBUG] Nessuna dipendenza assegnata.");
                break;
            case 1:
                AddDependency(DependencyType.Drugs);
                Debug.Log("[DEBUG] Assegnata dipendenza: DROGHE");
                break;
            case 2:
                AddDependency(DependencyType.Gambling);
                Debug.Log("[DEBUG] Assegnata dipendenza: GAMBLING");
                break;
            case 3:
                AddDependency(DependencyType.Internet);
                Debug.Log("[DEBUG] Assegnata dipendenza: INTERNET");
                break;
            default:
                Debug.LogWarning($"[DEBUG] Indice {charIndex} non gestito nello switch!");
                break;
        }
    }

    public void ApplyMovementMalus(GameObject player)
    {
        Debug.Log("[DEBUG] Il Portale ha chiamato ApplyMovementMalus!"); // <--- IL PORTALE CHIAMA?

        if (player == null)
        {
            Debug.LogError("[ERRORE] Il parametro 'player' passato al Manager è NULL!");
            return;
        }

        StartCoroutine(TemporaryMalusCoroutine(player));
    }

    IEnumerator TemporaryMalusCoroutine(GameObject player)
    {
        NewPlayerMovement playerMovement = player.GetComponent<NewPlayerMovement>();

        if (playerMovement == null)
        {
            Debug.LogError("[ERRORE] Non ho trovato lo script 'NewPlayerMovement' sul Player!");
            yield break;
        }

        IMovementStrategy strategyToApply = new NormalMovementStrategy();
        float currentDuration = 0f;
        bool malusFound = false;

        // Debug per vedere le flag attuali
        Debug.Log($"[DEBUG] Controllo Dipendenze. Stato attuale: {currentDependencies}");

        if (HasDependency(DependencyType.Drugs))
        {
            currentDuration = durationDrugs;
            malusFound = true;

            // Strategia di Test (Drunk) per essere sicuri che si veda
            strategyToApply = new DrunkStrategy();
            Debug.Log("[DEBUG] Trovata Flag DROGHE. Applico DrunkStrategy.");
        }
        else if (HasDependency(DependencyType.Gambling))
        {
            currentDuration = durationGambling;
            malusFound = true;
            strategyToApply = new GamblerStrategy();
            Debug.Log("[DEBUG] Trovata Flag GAMBLING.");
        }
        else if (HasDependency(DependencyType.Internet))
        {
            currentDuration = durationInternet;
            malusFound = true;
            strategyToApply = new InternetAddictStrategy();
            Debug.Log("[DEBUG] Trovata Flag INTERNET.");
        }

        if (!malusFound)
        {
            Debug.LogWarning("[DEBUG] Nessuna dipendenza attiva trovata. Il giocatore rimane normale.");
            yield break;
        }

        // APPLICAZIONE
        playerMovement.SetStrategy(strategyToApply);
        Debug.Log($"[DEBUG] Strategia applicata! Attendo {currentDuration} secondi...");

        yield return new WaitForSeconds(currentDuration);

        playerMovement.SetStrategy(new NormalMovementStrategy());
        Debug.Log("[DEBUG] Malus terminato. Ritorno alla normalità.");
    }

    public void AddDependency(DependencyType type) { currentDependencies |= type; }
    public void RemoveDependency(DependencyType type) { currentDependencies &= ~type; }
    public bool HasDependency(DependencyType type) { return (currentDependencies & type) != 0; }
}