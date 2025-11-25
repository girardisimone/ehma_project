using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class CharacterSelectionMenu : MonoBehaviour
{
    [Header("Riferimenti UI")]
    public Image characterDisplay;       // L'immagine grande del personaggio
    public Button selectButton;          // Il pulsante (assegna qui il tuo bottone con la tua sprite)

    [Header("Dati Personaggi")]
    public Sprite[] characterSprites;    // La lista delle immagini dei personaggi

    [Header("Colori Pulsante")]
    public Color normalColor = Color.white;   // Colore quando è selezionabile (Bianco = colore originale sprite)
    public Color selectedColor = Color.green; // Colore quando è GIÀ selezionato

    // Indici per la logica
    private int viewingIndex = 0; // Chi sto guardando
    private int lockedIndex = 0;  // Chi ho scelto davvero

    private void Start()
    {
        // Recupera l'ultimo salvataggio (o 0 se è la prima volta)
        lockedIndex = PlayerPrefs.GetInt("SelectedCharacter", 0);

        // Inizia mostrando il personaggio salvato
        viewingIndex = lockedIndex;

        UpdateUI();
    }

    // --- Navigazione (Frecce) ---

    public void NextCharacter()
    {
        viewingIndex++;
        if (viewingIndex >= characterSprites.Length)
            viewingIndex = 0;

        UpdateUI();
    }

    public void PreviousCharacter()
    {
        viewingIndex--;
        if (viewingIndex < 0)
            viewingIndex = characterSprites.Length - 1;

        UpdateUI();
    }

    // --- Azione di Selezione ---

    public void SelectCharacter()
    {
        lockedIndex = viewingIndex;

        // Salva la scelta in memoria
        PlayerPrefs.SetInt("SelectedCharacter", lockedIndex);
        PlayerPrefs.Save();

        Debug.Log($"[MENU] Hai selezionato il personaggio: {lockedIndex}");

        // Aggiorna la grafica per mostrare che è stato preso
        UpdateUI();
    }

    // --- Aggiornamento Grafico ---

    private void UpdateUI()
    {
        // 1. Aggiorna l'immagine centrale
        if (characterSprites.Length > 0)
        {
            characterDisplay.sprite = characterSprites[viewingIndex];
            characterDisplay.SetNativeSize(); // Mantiene le proporzioni giuste
        }

        // 2. Gestione Colore del Pulsante (La parte importante)

        // Otteniamo la configurazione colori attuale del bottone
        ColorBlock colors = selectButton.colors;

        if (viewingIndex == lockedIndex)
        {
            // --- CASO: PERSONAGGIO GIÀ SELEZIONATO ---

            // Diciamo al bottone: "Quando sei disabilitato, diventa VERDE"
            colors.disabledColor = selectedColor;
            colors.colorMultiplier = 1; // Assicura che il colore sia pieno
            selectButton.colors = colors; // Applica le modifiche

            // Disabilitiamo il bottone -> Unity applicherà automaticamente il 'disabledColor' (Verde)
            selectButton.interactable = false;
        }
        else
        {
            // --- CASO: PERSONAGGIO NUOVO (DA SELEZIONARE) ---

            // Diciamo al bottone: "Il tuo colore normale è BIANCO"
            colors.normalColor = normalColor;
            colors.highlightedColor = new Color(0.9f, 0.9f, 0.9f, 1); // Leggero effetto hover
            selectButton.colors = colors; // Applica le modifiche

            // Abilitiamo il bottone -> Unity applicherà il 'normalColor' (Bianco)
            selectButton.interactable = true;
        }
    }

    public void PlayGame()
    {
        // Carica la scena di gioco
        SceneManager.LoadScene("SampleScene");
    }
}