using UnityEngine;
using System.Collections.Generic;

public class PortalManager : MonoBehaviour
{
    // --- FLAG LOGICO ---
    [Header("🔧 Logica Trigger")]
    [Tooltip("Se true, le griglie estratte vengono rimosse finché la lista non si svuota.")]
    private bool flagPerTrigger = true;
    
    public static PortalManager Instance { get; private set; }

    [Header("⚙️ Impostazioni Globali")]
    [Tooltip("Costo base di utilizzo di un portale, se non specificato diversamente.")]
    public int defaultCost = 5;

    [Tooltip("Se attivo, i portali verranno collegati in modo casuale.")]
    public bool randomConnections = false;
    
    [Header("📜 Elenco Portali (auto compilato)")]
    public List<PortalTeleporter> allPortals = new List<PortalTeleporter>();
    
    [Header("🎲 Probabilità Standard (Fase 2 o Timeout)")]
    [Tooltip("Griglie usate quando finiscono quelle iniziali o scade il tempo senza averle finite")]
    public List<GridProbability> gridStandardProbabilities = new List<GridProbability>();
    
    [Header("🎲 Probabilità Iniziali (Fase 1 - Trigger)")]
    [Tooltip("Lista iniziale che si svuota man mano")]
    public List<GridProbability> initialGridProbabilities = new List<GridProbability>();
    
    // Lista attiva usata logicamente dallo script
    private List<GridProbability> gridProbabilities = new List<GridProbability>();

    
    // --- NUOVE VARIABILI PER IL TIMER ---
    [Header("⏰ Evento Temporale (Cambio Probabilità)")]
    [Tooltip("Dopo quanti secondi dall'avvio devono cambiare le probabilità?")]
    public float secondsBeforeChange = 60f; 

    [Tooltip("lista di prob per per arrivare alla principessa (Solo se sequenza iniziale completata)")]
    public List<GridProbability> lateGameProbabilities = new List<GridProbability>();

    [Tooltip("lista di prob per per arrivare al samurai")]
    public List<GridProbability> toSamuraiGridProbalities = new List<GridProbability>();

    private float timer = 0f;
    private bool probabilitiesChanged = false;

    private bool toSamuraiProbabilitiesChanged = false;
    // -------------------------------------

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        // 🟢 INIZIALIZZAZIONE IMPORTANTE:
        if (initialGridProbabilities.Count > 0)
        {
            gridProbabilities = new List<GridProbability>(initialGridProbabilities);
        }
        else
        {
            Debug.LogWarning("Nessuna Initial Probability impostata, carico le Standard.");
            gridProbabilities = new List<GridProbability>(gridStandardProbabilities);
            flagPerTrigger = false; 
        }
    }

    private void Update()
    {
        if (!probabilitiesChanged)
        {
            timer += Time.deltaTime;

            if (timer >= secondsBeforeChange)
            {
                CambiaProbabilita();
            }
        }
    }

    /// <summary>
    /// Gestisce il cambio probabilità allo scadere del timer.
    /// </summary>
    private void CambiaProbabilita()
    {
        // --- MODIFICA RICHIESTA ---
        // Se flagPerTrigger è ancora true, significa che il giocatore NON ha finito la sequenza iniziale
        // (o non ha mai preso portali). In questo caso, passiamo alle STANDARD, non alla Principessa.
        if (flagPerTrigger)
        {
            // Carichiamo le standard come fallback
            gridProbabilities = new List<GridProbability>(gridStandardProbabilities);
            
            // Disabilitiamo il trigger flag (così smette di rimuovere elementi)
            flagPerTrigger = false; 
            
            Debug.Log($"⏰ TIMER SCADUTO! Il giocatore non ha completato la sequenza iniziale. Passaggio a probabilità STANDARD (Niente Principessa).");
        }
        else 
        {
            // Se flagPerTrigger è false, il giocatore ha finito la sequenza iniziale correttamente.
            // Possiamo procedere con il Late Game (Principessa).
            if (lateGameProbabilities != null && lateGameProbabilities.Count > 0)
            {
                gridProbabilities = lateGameProbabilities; 
                Debug.Log($"⏰ TIMER SCADUTO! Sequenza completata. Le probabilità ora puntano alla PRINCIPESSA (Late Game).");
            }
            else
            {
                Debug.LogWarning("Timer scaduto e sequenza finita, ma 'lateGameProbabilities' è vuota!");
            }
        }

        // In entrambi i casi, segniamo che il cambio temporale è avvenuto
        probabilitiesChanged = true; 
    }
    
    public void CambiaProbabilitaToSamurai()
    {
        if (toSamuraiGridProbalities != null && toSamuraiGridProbalities.Count > 0 && toSamuraiProbabilitiesChanged  == false )
        {
            gridProbabilities = toSamuraiGridProbalities; 
            toSamuraiProbabilitiesChanged = true;
            
            // Disabilitiamo il trigger flag se andiamo dal samurai
            flagPerTrigger = false;
            
            Debug.Log($"Cambio delle probabilità per arrivare nella griglia del samurai");
        }
        else
        {
            Debug.LogWarning("Attivato Samurai, ma la lista 'toSamuraiGridProbalities' è vuota!");
            toSamuraiProbabilitiesChanged = true;
        }
    }

    public void RegisterPortal(PortalTeleporter portal)
    {
        if (!allPortals.Contains(portal))
            allPortals.Add(portal);
    }
    
    public void SetAllCosts(int newCost)
    {
        foreach (var p in allPortals)
            p.setTravelCost(newCost);
    }

    public Grid getRandomGrid()
    {   
        if (gridProbabilities == null || gridProbabilities.Count == 0)
        {
            Debug.LogWarning($"GridProbabilities è vuota! Ritorno null.");
            return null;
        }

        int totalWeight = 0;
        foreach (var item in gridProbabilities)
            totalWeight += Mathf.Max(item.probability, 0); 

        if (totalWeight == 0)
            return null; 

        int randomValue = Random.Range(0, totalWeight);

        for (int i = 0; i < gridProbabilities.Count; i++)
        {
            var item = gridProbabilities[i];

            if (randomValue < item.probability)
            {
                Debug.Log($"Estratta la griglia {item.grid} (Probabilità attuale: {item.probability})");
                Grid selectedGrid = item.grid;

                // --- LOGICA TRIGGER: RIMOZIONE E SWITCH ---
                if (flagPerTrigger)
                {
                    gridProbabilities.RemoveAt(i);
                    // Debug.Log($"[Trigger] Rimosso elemento. Rimasti: {gridProbabilities.Count}");

                    if (gridProbabilities.Count == 0)
                    {
                        Debug.Log("🎉 Lista Iniziale esaurita! Passaggio alle Probabilità Standard.");
                        gridProbabilities = new List<GridProbability>(gridStandardProbabilities);
                        flagPerTrigger = false;
                    }
                }
                // ------------------------------------------

                return selectedGrid;
            }

            randomValue -= item.probability;
        }
       
        return gridProbabilities[gridProbabilities.Count - 1].grid;
    }

    public bool isRandomConnections()
    {
        return randomConnections;
    }
}