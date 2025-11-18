using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    [Header("Link alle 'cartelle' nella gerarchia che \n contengono portali e gemme di questa grid")]
    public Transform Portals;   // ← trascina qui l'oggetto "Portals"
    public Transform Gem;       // ← trascina qui l'oggetto "Gem"

    [Header("Liste generate automaticamente durante il gioco \n prendendo i  componenti nella cartella \n indicata sopra")]
    public List<PortalTeleporter> portalsList = new List<PortalTeleporter>();
    public List<Gem> gemsList = new List<Gem>();


    private void UpdateLists()
    {
        portalsList.Clear();
        gemsList.Clear();
        
        // --- Popola lista Portals ---
        if (Portals != null)
        {
            foreach (Transform child in Portals)
            {
                PortalTeleporter p = child.GetComponent<PortalTeleporter>();
                if (p != null)
                {
                    portalsList.Add(p);
                }
            }
        }

        // --- Popola lista Gem ---
        if (Gem != null)
        {
            foreach (Transform child in Gem)
            {
                Gem g = child.GetComponent<Gem>();
                if (g != null)
                {
                    gemsList.Add(g);
                }
            }
        }
    }

    public void Start()
    {
        UpdateLists();
    }
    private void Awake()
    {
        UpdateLists();
    }

    public PortalTeleporter getDestinationPortal()
    {
        foreach (PortalTeleporter p in  portalsList )
        {
            if (p.isDestinationPortal())
            {
                return p;
            }
        }
        Debug.LogWarning($"Non c'è un portale di arrivo nella griglia selezionata come destinazione del portale");
        return null;
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (!Application.isPlaying)
        {
            UpdateLists();
        }
    }
#endif
}