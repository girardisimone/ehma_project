using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class CharacterSelectionMenu : MonoBehaviour
{
    [Header("Configurazione")]
    public Image characterDisplay;       // L'immagine UI che cambia
    public Sprite[] characterSprites;    // Trascina qui gli sprite dei personaggi (es. Idle)

    private int selectedIndex = 0;

    private void Start()
    {
        UpdateUI();
    }

    public void NextCharacter()
    {
        selectedIndex++;
        if (selectedIndex >= characterSprites.Length)
            selectedIndex = 0; // Torna al primo

        UpdateUI();
    }

    public void PreviousCharacter()
    {
        selectedIndex--;
        if (selectedIndex < 0)
            selectedIndex = characterSprites.Length - 1; // Va all'ultimo

        UpdateUI();
    }

    private void UpdateUI()
    {
        // Cambia l'immagine nel menu
        characterDisplay.sprite = characterSprites[selectedIndex];
        // Mantieni le proporzioni originali dello sprite
        characterDisplay.SetNativeSize();
    }

    public void PlayGame()
    {
        // SALVA LA SCELTA!
        // PlayerPrefs è un modo semplice per salvare dati tra scene
        PlayerPrefs.SetInt("SelectedCharacter", selectedIndex);

        // Carica il gioco (assicurati che il nome sia giusto)
        SceneManager.LoadScene("SampleScene");
    }
}