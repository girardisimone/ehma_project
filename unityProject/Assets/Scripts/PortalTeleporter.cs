using UnityEngine;
using TMPro;
using UnityEngine.UI; // Necessario per l'interazione con i Button

public class PortalTeleporter : MonoBehaviour
{
    // --- Configurazione del Portale ---
    public PortalTeleporter destinationPortal;
    public int travelCost = 5;

    // Variabile per evitare teletrasporti immediati di ritorno.
    private bool isPlayerInside = false;

    // Riferimento al Player (viene assegnato in OnTriggerEnter2D)
    private Transform player;

    // --- Riferimenti UI (Trascina questi oggetti nell'Inspector) ---
    [Header("Riferimenti UI di Conferma")]
    [Tooltip("Il pannello UI che contiene il messaggio e i pulsanti.")]
    public GameObject confirmationPanel;
    [Tooltip("Il TextMeshPro che mostra il costo del portale.")]
    public TextMeshProUGUI costText;
    [Tooltip("Il riferimento al componente Button (Pulsante SI)")]
    public Button confirmButton;

    // --- Riferimenti al Player (per disabilitare il movimento) ---
    private MonoBehaviour playerMovementScript; // Usa il tipo base per flessibilità
    private const string PLAYER_TAG = "Player";


    private void Awake()
    {
        // Se il pannello di conferma non è attivo all'inizio, lo disattiviamo
        if (confirmationPanel != null)
        {
            confirmationPanel.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 1. Controlla il Tag e la bandiera anti-loop
        if (other.CompareTag(PLAYER_TAG) && !isPlayerInside)
        {
            player = other.transform; // Memorizza il Player
            isPlayerInside = true;    // Impedisce ri-attivazione immediata

            int currentGems = ScoreManager.GemCount;

            // Tentiamo di trovare lo script di movimento sul Player
            playerMovementScript = other.GetComponent<MonoBehaviour>(); // Assumi che il Player abbia un componente di movimento che può essere disabilitato

            if (currentGems >= travelCost)
            {
                // *** 2. MOSTRA LA UI DI CONFERMA ***

                // Disabilita il movimento del Player
                SetPlayerMovement(false);

                // Aggiorna il testo e mostra il pannello
                costText.text = $"Cost: {travelCost} Gems. Do you want to use the portal?";
                confirmationPanel.SetActive(true);

                // Aggiungiamo un listener dinamico al pulsante 'Sì'
                // Rimuoviamo il listener precedente (se esiste) per evitare chiamate multiple
                confirmButton.onClick.RemoveAllListeners();
                confirmButton.onClick.AddListener(ConfirmTeleport);
            }
            else
            {
                // Se non ci sono abbastanza gemme, mostriamo un messaggio (opzionale)
                Debug.Log($"Gemme insufficienti! Costo: {travelCost}, possedute: {currentGems}.");
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag(PLAYER_TAG))
        {
            // Quando il giocatore esce, resetta la bandiera anti-loop.
            isPlayerInside = false;
        }
    }

    // --- Metodo di Teletrasporto Chiamato dal Pulsante UI ---

    public void ConfirmTeleport()
    {
        int currentGems = ScoreManager.GemCount;

        // Nasconde la UI di conferma
        confirmationPanel.SetActive(false);

        if (currentGems >= travelCost)
        {
            // *** PAGAMENTO E TELETRASPORTO ESEGUITO ***

            // 1. Sottrai il costo
            ScoreManager.GemCount -= travelCost;

            // 2. Aggiorna la UI del punteggio
            if (ScoreManager.Instance != null)
            {
                ScoreManager.Instance.UpdateScoreText(ScoreManager.GemCount);
            }

            // 3. Esegui il Teletrasporto
            if (destinationPortal != null && player != null)
            {
                destinationPortal.isPlayerInside = true;
                player.position = destinationPortal.transform.position;

                // 4. Notifica la Difficoltà
                if (DifficultyManager.Instance != null)
                {
                    DifficultyManager.Instance.RegisterPortalUse();
                }

                // 5. DISTRUGGI PORTALE DI DESTINAZIONE
                Destroy(destinationPortal.gameObject);

                Debug.Log($"Teletrasporto completato! Costo: {travelCost}.");
            }
            else
            {
                Debug.LogError("Errore durante il teletrasporto: Player o Destinazione non validi.");
            }
        }

        // Ri-abilita il movimento del Player in ogni caso
        SetPlayerMovement(true);
    }

    // --- Metodo Chiamato dal Pulsante NO ---

    public void CancelTeleport()
    {
        // Nasconde la UI di conferma
        confirmationPanel.SetActive(false);

        // Ri-abilita il movimento del Player
        SetPlayerMovement(true);

        // Rimuove la bandiera anti-loop (per consentire al Player di rientrare subito)
        isPlayerInside = false;

        Debug.Log("Teletrasporto annullato.");
    }

    // --- Helper per la gestione del movimento del Player ---
    private void SetPlayerMovement(bool enable)
    {
        if (playerMovementScript != null)
        {
            // Assumi che il PlayerMovementScript sia il primo MonoBehaviour trovato che gestisce il movimento.
            // Se usi un nome specifico (es. 'PlayerController'), cambialo qui:
            // var playerController = player.GetComponent<PlayerController>();
            // if (playerController != null) playerController.enabled = enable;

            playerMovementScript.enabled = enable;
        }
    }
}