using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Text.RegularExpressions; // Serve per leggere i numeri in modo intelligente

public class MiniMapController : MonoBehaviour
{
    public static MiniMapController Instance;

    [Header("Collegamenti UI")]
    public Transform gridContainer;
    public RectTransform playerMarker;

    [Header("Impostazioni Colori")]
    public Color visitedColor = Color.gray;

    private List<Image> mapCells = new List<Image>();

    void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }

        mapCells.Clear();
        foreach (Transform child in gridContainer)
        {
            Image cellImage = child.GetComponent<Image>();
            if (cellImage != null) mapCells.Add(cellImage);
        }
    }

    System.Collections.IEnumerator Start()
    {
        yield return null; 
        // All'avvio proviamo ad aggiornare, ma senza crashare se fallisce
        UpdateMiniMap("A1");
    }

    public void UpdateMiniMap(string rawName)
    {
        if (string.IsNullOrEmpty(rawName)) return;

        // 1. PULIZIA DEL NOME (Magia!)
        // Se arriva "Grid A1" o "Grid_B2", teniamo solo le ultime 2 lettere/numeri
        // Esempio: "Grid A1" diventa "A1"
        string cleanID = ExtractGridID(rawName);

        int index = CalculateIndex(cleanID);

        if (index >= 0 && index < mapCells.Count)
        {
            playerMarker.position = mapCells[index].rectTransform.position;
            mapCells[index].color = visitedColor;
        }
        // Rimuoviamo il log di errore per non intasare la console, lasciamo solo se serve
    }

    // Funzione che estrae "A1" da qualsiasi stringa ("Grid A1", "Room_A1", ecc)
    private string ExtractGridID(string name)
    {
        // Cerca una lettera (A-D) seguita da un numero (1-4)
        var match = Regex.Match(name, @"([A-D])([1-4])", RegexOptions.IgnoreCase);
        if (match.Success)
        {
            return match.Value.ToUpper(); // Restituisce es. "A1"
        }
        return "ERROR";
    }

    private int CalculateIndex(string id)
    {
        if (id == "ERROR") return -1;

        try 
        {
            char lettera = id[0]; // 'C'
            int numero = int.Parse(id.Substring(1)); // 2

            int colonna = char.ToUpper(lettera) - 'A'; 
            int riga = numero - 1;

            return (riga * 4) + colonna;
        }
        catch
        {
            return -1; // Se qualcosa va storto, non crashare il gioco
        }
    }
}