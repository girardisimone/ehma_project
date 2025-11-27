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

    [Header("Impostazioni Colori")]
    public Color visitedColor = Color.gray; // Il colore interno (Grigio)
    public Color borderColor = Color.black; // Il colore della cornice (Nero)
    
    [Header("Spessore Bordo")]
    public int borderThickness = 3; // Quanto deve essere spesso il bordo nero

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
        // UpdateMiniMap("A1"); // Decommenta se vuoi forzare l'avvio su A1
    }

    public void UpdateMiniMap(string rawName)
    {
        if (string.IsNullOrEmpty(rawName)) return;

        string cleanID = ExtractGridID(rawName);
        int index = CalculateIndex(cleanID);

        if (index >= 0 && index < mapCells.Count)
        {
            Image targetCell = mapCells[index];

            // 1. Sposta il player
            playerMarker.position = targetCell.rectTransform.position;
            
            // =========================================================
            // 2. TRUCCO DEL BORDO INTERNO
            // =========================================================
            
            // Controlliamo se abbiamo già trasformato questa cella
            // (Se ha dei figli, vuol dire che l'abbiamo già colorata)
            if (targetCell.transform.childCount == 0)
            {
                // A. Trasformiamo la cella principale nel BORDO NERO
                targetCell.color = borderColor;

                // B. Creiamo un nuovo oggetto dentro che sarà il vero GRIGIO
                GameObject innerObj = new GameObject("InnerColor");
                innerObj.transform.SetParent(targetCell.transform, false);

                // C. Aggiungiamo l'immagine e la coloriamo di GRIGIO
                Image innerImage = innerObj.AddComponent<Image>();
                innerImage.color = visitedColor;

                // D. Impostiamo le dimensioni per lasciare lo spazio al bordo
                RectTransform rt = innerObj.GetComponent<RectTransform>();
                
                // Ancoraggio totale (Stretch) su tutti i lati
                rt.anchorMin = Vector2.zero;
                rt.anchorMax = Vector2.one;
                
                // Margini (Padding) -> Questo crea lo spessore del bordo visivo!
                // Sinistra, Basso, Destra, Alto
                rt.offsetMin = new Vector2(borderThickness, borderThickness); 
                rt.offsetMax = new Vector2(-borderThickness, -borderThickness);
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

    private int CalculateIndex(string id)
    {
        if (id == "ERROR") return -1;
        try 
        {
            char lettera = id[0];
            int numero = int.Parse(id.Substring(1));
            int colonna = char.ToUpper(lettera) - 'A'; 
            int riga = numero - 1;
            return (riga * 4) + colonna;
        }
        catch { return -1; }
    }
}