using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Text.RegularExpressions; 

public class MiniMapController : MonoBehaviour
{
    public static MiniMapController Instance;

    [Header("Collegamenti UI")]
    public Transform gridContainer;
    public RectTransform playerMarker;

    [Header("Colori Scacchiera")]
    // Scegli due tonalità di grigio diverse nell'Inspector
    public Color colorLight = new Color(0.8f, 0.8f, 0.8f, 1f); // Grigio Chiaro
    public Color colorDark = new Color(0.5f, 0.5f, 0.5f, 1f);  // Grigio Scuro

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
        // UpdateMiniMap("A1"); // Decommenta se vuoi l'avvio forzato su A1
    }

    public void UpdateMiniMap(string rawName)
    {
        if (string.IsNullOrEmpty(rawName)) return;

        string cleanID = ExtractGridID(rawName);
        
        // Calcoliamo indice, riga e colonna per la scacchiera
        int col = -1;
        int row = -1;
        int index = CalculateIndexAndCoords(cleanID, out col, out row);

        if (index >= 0 && index < mapCells.Count)
        {
            Image targetCell = mapCells[index];
            
            // 1. Sposta il player
            playerMarker.position = targetCell.rectTransform.position;

            // 2. LOGICA SCACCHIERA
            // Se la somma di riga + colonna è pari, usa un colore, altrimenti l'altro.
            // Questo crea l'alternanza perfetta anche su griglie pari (4x4).
            bool isEven = (col + row) % 2 == 0;

            if (isEven)
            {
                targetCell.color = colorLight;
            }
            else
            {
                targetCell.color = colorDark;
            }
        }
    }

    // --- FUNZIONI DI SUPPORTO ---
    private string ExtractGridID(string name)
    {
        var match = Regex.Match(name, @"([A-D])([1-4])", RegexOptions.IgnoreCase);
        if (match.Success) return match.Value.ToUpper();
        return "ERROR";
    }

    // Ho modificato leggermente questa funzione per restituirci anche colonna e riga
    private int CalculateIndexAndCoords(string id, out int colOut, out int rowOut)
    {
        colOut = -1;
        rowOut = -1;

        if (id == "ERROR") return -1;
        try 
        {
            char lettera = id[0];
            int numero = int.Parse(id.Substring(1));

            colOut = char.ToUpper(lettera) - 'A'; 
            rowOut = numero - 1;

            return (rowOut * 4) + colOut;
        }
        catch { return -1; }
    }
}