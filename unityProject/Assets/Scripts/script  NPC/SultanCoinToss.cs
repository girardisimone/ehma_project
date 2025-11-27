using UnityEngine;
using TMPro; // Serve per modificare il testo
using System.Collections;

public class SultanCoinToss : MonoBehaviour
{
    [Header("Componenti UI")]
    public GameObject popupWindow;      // L'oggetto padre di tutto il popup
    public TextMeshProUGUI messageText; // Il testo del messaggio
    public GameObject buttonAccept;     // Il tasto verde
    public GameObject buttonRefuse;     // Il tasto rosso

    [Header("Destinazioni")]
    public Transform forwardLocation;   // Dove va se esce TESTA (Vince)
    public Transform backLocation;      // Dove va se esce CROCE (Perde)

    // Messaggio iniziale da rimettere quando il popup si riapre
    private string originalMessage = "Do you want to play Heads or Tails? \nHeads: move forward. Tails: go back.";

    void OnEnable()
    {
        // Ogni volta che il popup si apre, resetta il testo e i bottoni
        if (messageText != null) messageText.text = originalMessage;
        if (buttonAccept != null) buttonAccept.SetActive(true);
        if (buttonRefuse != null) buttonRefuse.SetActive(true);
    }

    // --- FUNZIONE COLLEGATA AL TASTO "ACCEPT" ---
    public void OnPlayClicked()
    {
        // 1. Nascondi i bottoni per non far cliccare due volte
        buttonAccept.SetActive(false);
        buttonRefuse.SetActive(false);

        // 2. Calcola il risultato (50% e 50%)
        // Random.value dà un numero tra 0.0 e 1.0. Se è maggiore di 0.7 è Testa.
        bool isHeads = Random.value > 0.7f;

        // 3. Avvia la sequenza del risultato
        StartCoroutine(ShowResultAndTeleport(isHeads));
    }

    // --- FUNZIONE COLLEGATA AL TASTO "NO ACCEPT" ---
    public void OnRefuseClicked()
    {
        // Chiude semplicemente il popup
        popupWindow.SetActive(false);
    }

    // --- LA SEQUENZA DI GIOCO ---
    IEnumerator ShowResultAndTeleport(bool isHeads)
    {
        // Mostra il risultato a video in INGLESE
        if (isHeads)
            messageText.text = "It's... HEADS!\n(You win!)";
        else
            messageText.text = "It's... TAILS!\n(You lose!)";

        // Il resto del codice rimane uguale...
        yield return new WaitForSeconds(2f);

    // Trova il player
    GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            if (isHeads)
            {
                // Teletrasporta alla posizione Avanti
                player.transform.position = forwardLocation.position;
            }
            else
            {
                // Teletrasporta alla posizione Indietro
                player.transform.position = backLocation.position;
            }
        }

        // Chiudi il popup
        popupWindow.SetActive(false);
    }
}