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

    [Header("⚙️ Default Portal Settings")]
    [Tooltip("Modalità di default per tutti i portali.")]
    public Portal.ActivationMode defaultActivationMode = Portal.ActivationMode.Automatic;

    [Header("📜 Elenco Portali (auto compilato)")]
    public List<Portal> allPortals = new List<Portal>();
    
    [Header("Probabilità di teletrasporto per ogni griglia")]
    public List<GridProbability> gridProbabilities = new List<GridProbability>();

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
        SetDefaultActivationMode();
    }

    /// <summary>
    /// Registra un portale nella lista globale.
    /// </summary>
    public void RegisterPortal(Portal portal)
    {
        if (!allPortals.Contains(portal))
            allPortals.Add(portal);
    }
    
    /// <summary>
    /// Imposta la modalità di attivazione di default per tutti i portali.
    /// </summary>
    public void SetDefaultActivationMode()
    {
        foreach (var portal in allPortals)
        {
            portal.SetActivationMode(defaultActivationMode);
        }
        Debug.Log($"PortalManager: impostata modalità {defaultActivationMode} a tutti i portali.");
    }

    // --- METODI GLOBALI DI CONTROLLO ---

    public void SetAllCosts(int newCost)
    {
        foreach (var p in allPortals)
            p.SetTravelCost(newCost);
    }

    public void ActivateAll(bool value)
    {
        foreach (var p in allPortals)
            p.SetActive(value);
    }

    public void SetAllModes(Portal.ActivationMode mode)
    {
        foreach (var p in allPortals)
            p.SetActivationMode(mode);
    }

    public void SetAllTypes(Portal.PortalType type)
    {
        foreach (var p in allPortals)
            p.SetType(type);
    }
    
    
    /// <summary>
    /// Restituisce una Grid selezionata casualmente in base alle probabilità intere
    /// </summary>
    public Grid getRandomGrid()
    {
        if (gridProbabilities == null || gridProbabilities.Count == 0)
            return null;

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
                return item.grid;

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

