using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class CharacterSelectionMenu : MonoBehaviour
{
    [Header("Riferimenti UI")]
    public Image characterDisplay;       // L'immagine grande del personaggio
    public Button selectButton;          // Il pulsante "SELEZIONA"
    public TextMeshProUGUI storyText;    // <--- NUOVO: Trascina qui il testo della storia

    [Header("Dati Personaggi")]
    public Sprite[] characterSprites;    // Le immagini

    [Header("Storie Personaggi")]
    // TextArea crea un box grande nell'Inspector (Min 3 righe, Max 10 righe)
    [TextArea(3, 10)]
    public string[] characterStories;    // <--- NUOVO: Scrivi qui le storie in ordine

    [Header("Colori Pulsante")]
    public Color normalColor = Color.white;
    public Color selectedColor = Color.green;

    // Indici
    private int viewingIndex = 0;
    private int lockedIndex = 0;

    private void Start()
    {
        // Recupera l'ultimo salvataggio
        lockedIndex = PlayerPrefs.GetInt("SelectedCharacter", 0);
        viewingIndex = lockedIndex;

        UpdateUI();
    }

    // --- Navigazione ---

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

    // --- Selezione ---

    public void SelectCharacter()
    {
        lockedIndex = viewingIndex;

        PlayerPrefs.SetInt("SelectedCharacter", lockedIndex);
        PlayerPrefs.Save();

        Debug.Log($"[MENU] Hai selezionato il personaggio: {lockedIndex}");

        UpdateUI();
    }

    // --- Aggiornamento Grafico ---

    private void UpdateUI()
    {
        // 1. Aggiorna l'immagine
        if (characterSprites.Length > viewingIndex)
        {
            characterDisplay.sprite = characterSprites[viewingIndex];
            characterDisplay.SetNativeSize();
        }

        // 2. AGGIORNA LA STORIA (NUOVO)
        if (storyText != null && characterStories.Length > viewingIndex)
        {
            storyText.text = characterStories[viewingIndex];
        }

        // 3. Gestione Colore del Pulsante
        ColorBlock colors = selectButton.colors;

        if (viewingIndex == lockedIndex)
        {
            // GIÀ SELEZIONATO
            colors.disabledColor = selectedColor;
            colors.colorMultiplier = 1;
            selectButton.colors = colors;
            selectButton.interactable = false;
        }
        else
        {
            // DA SELEZIONARE
            colors.normalColor = normalColor;
            colors.highlightedColor = new Color(0.9f, 0.9f, 0.9f, 1);
            selectButton.colors = colors;
            selectButton.interactable = true;
        }
    }

    public void PlayGame()
    {
        // Salva per sicurezza anche quello che stai guardando se premi Gioca diretto
        // (Opzionale, se vuoi forzare la selezione manuale togli queste due righe sotto)
        // PlayerPrefs.SetInt("SelectedCharacter", lockedIndex);
        // PlayerPrefs.Save();

        SceneManager.LoadScene("SampleScene");
    }
}