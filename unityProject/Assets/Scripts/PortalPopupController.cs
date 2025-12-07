using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PortalPopupController : MonoBehaviour
{
    public static PortalPopupController Instance;

    [Header("Riferimenti UI")]
    public GameObject popupPanel;
    public TextMeshProUGUI messageText;
    public Button usePortalButton;
    public Button continueButton;

    private PortalTeleporter currentPortal;

    // Riferimento al movimento del player per riattivarlo se annulla
    private NewPlayerMovement playerMovementScript;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        Hide();
    }

    public void Show(PortalTeleporter portal, bool canUsePortal, string costDescription)
    {
        currentPortal = portal;

        // Troviamo il player
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            playerMovementScript = playerObj.GetComponent<NewPlayerMovement>();
        }

        popupPanel.SetActive(true);

        if (canUsePortal)
        {
            // --- CASO 1: PUÒ PAGARE ---
            // Il player resta bloccato finché non sceglie Si o No

            messageText.text = $"Do you want to use the portal? {costDescription}";

            usePortalButton.gameObject.SetActive(true);
            continueButton.gameObject.SetActive(true); // Assicurati che il tasto "No" sia visibile
        }
        else
        {
            // --- CASO 2: NON PUÒ PAGARE ---
            // Mostriamo il messaggio ma SBLOCCHIAMO subito il movimento

            messageText.text = $"You don't have enough resources \n {costDescription}";

            // Nascondiamo i pulsanti perché non c'è nulla da cliccare
            usePortalButton.gameObject.SetActive(false);
            continueButton.gameObject.SetActive(false);

            // *** MODIFICA FONDAMENTALE ***
            // Riattiviamo subito il movimento così il player può andarsene
            if (playerMovementScript != null)
            {
                playerMovementScript.enabled = true;
            }
        }
    }

    public void OnUsePortalButton()
    {
        if (currentPortal != null)
        {
            currentPortal.TeleportPlayerAndPay();
        }

        Hide();
    }

    public void OnContinueButton()
    {
        // Se annulla, dobbiamo riabilitare il movimento del player
        if (playerMovementScript != null)
        {
            playerMovementScript.enabled = true;
        }
        Hide();
    }

    public void Hide()
    {
        popupPanel.SetActive(false);
        currentPortal = null;
        playerMovementScript = null;
    }

    public void HideIfCurrent(PortalTeleporter portal)
    {
        if (currentPortal == portal)
        {
            Hide();
        }
    }
}