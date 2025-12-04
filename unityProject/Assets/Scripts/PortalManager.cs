using UnityEngine;
using System.Collections.Generic;

public class PortalManager : MonoBehaviour
{
    public static PortalManager Instance { get; private set; }

    [Header("⚙️ Impostazioni Globali")]
    [Tooltip("Costo base di utilizzo di un portale, se non specificato diversamente.")]
    public int defaultCost = 5;

    [Tooltip("Se attivo, i portali verranno collegati in modo casuale.")]
    public bool randomConnections = false;
    
    [Header("📜 Elenco Portali (auto compilato)")]
    public List<PortalTeleporter> allPortals = new List<PortalTeleporter>();
    
    [Header("🎲 Probabilità Iniziali")]
    public List<GridProbability> gridProbabilities = new List<GridProbability>();

    // --- NUOVE VARIABILI PER IL TIMER ---
    [Header("⏰ Evento Temporale (Cambio Probabilità)")]
    [Tooltip("Dopo quanti secondi dall'avvio devono cambiare le probabilità?")]
    public float secondsBeforeChange = 60f; 

    [Tooltip("La NUOVA lista di probabilità che verrà usata scaduto il tempo.")]
    public List<GridProbability> lateGameProbabilities = new List<GridProbability>();

    private float timer = 0f;
    private bool probabilitiesChanged = false;
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
        // Il timer parte da 0 automaticamente
    }

    // --- AGGIUNTO UPDATE PER GESTIRE IL TEMPO ---
    private void Update()
    {
        // Se il cambio non è ancora avvenuto...
        if (!probabilitiesChanged)
        {
            // ...aumenta il timer
            timer += Time.deltaTime;

            // Se abbiamo superato il tempo limite
            if (timer >= secondsBeforeChange)
            {
                CambiaProbabilita();
            }
        }
    }

    /// <summary>
    /// Scambia la lista attuale con quella nuova "precisa"
    /// </summary>
    private void CambiaProbabilita()
    {
        if (lateGameProbabilities != null && lateGameProbabilities.Count > 0)
        {
            gridProbabilities = lateGameProbabilities; // Sovrascrive la lista attuale con quella nuova
            probabilitiesChanged = true; // Evita che il cambio avvenga di nuovo
            Debug.Log($"⏰ TIMER SCADUTO! Le probabilità dei portali sono cambiate, ora è molto probabile incontrare la principessa prendendo i portali.");
        }
        else
        {
            Debug.LogWarning("È scattato il timer, ma la lista 'lateGameProbabilities' è vuota!");
            probabilitiesChanged = true; // Blocchiamo comunque per non spammare l'errore
        }
    }

    /// <summary>
    /// Registra un portale nella lista globale.
    /// </summary>
    public void RegisterPortal(PortalTeleporter portal)
    {
        if (!allPortals.Contains(portal))
            allPortals.Add(portal);
    }
    
    // --- METODI GLOBALI DI CONTROLLO ---
    public void SetAllCosts(int newCost)
    {
        foreach (var p in allPortals)
            p.setTravelCost(newCost);
    }

    /// <summary>
    /// Restituisce una Grid selezionata casualmente in base alle probabilità intere
    /// </summary>
    public Grid getRandomGrid()
    {   
        // Debug.Log($"Estrazione di una griglia casuale di destinazione..."); 
        if (gridProbabilities == null || gridProbabilities.Count == 0)
        {
            Debug.LogWarning($"Non sono state inserite le griglie e le probabilità nell'oggetto PortalManager");
            return null;
        }

        // 1️⃣ Somma totale dei pesi
        int totalWeight = 0;
        foreach (var item in gridProbabilities)
            totalWeight += Mathf.Max(item.probability, 0); // evita valori negativi

        if (totalWeight == 0)
            return null; // tutte le probabilità a 0

        // 2️⃣ Numero casuale tra 0 e totalWeight-1
        int randomValue = Random.Range(0, totalWeight);

        // 3️⃣ Estrazione pesata
        foreach (var item in gridProbabilities)
        {
            if (randomValue < item.probability)
            {
                Debug.Log($"Estratta la griglia {item.grid} (Probabilità attuale: {item.probability})");
                return item.grid;
            }

            randomValue -= item.probability;
        }
       
        // fallback (non dovrebbe mai succedere)
        return gridProbabilities[gridProbabilities.Count - 1].grid;
    }

    public bool isRandomConnections()
    {
        return randomConnections;
    }
}