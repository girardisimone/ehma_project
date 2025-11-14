using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    [Header("Link ai contenitori nella gerarchia che \n contengono portali e gemme di questa grid")]
    public Transform Portals;   // ← trascina qui l'oggetto "Portals"
    public Transform Gem;       // ← trascina qui l'oggetto "Gem"

    [Header("Liste generate automaticamente")]
    public List<Portal> portalsList = new List<Portal>();
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
                Portal p = child.GetComponent<Portal>();
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


    private void Awake()
    {
        UpdateLists();
    }

    public Portal getDestinationPortal()
    {
        foreach (Portal p in  portalsList )
        {
            if (p.isDestinationPortal())
            {
                return p;
            }
        }
        Debug.Log($"non c'è un portale di arrivo nella griglia selezionata come destinazione del portale");
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