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

    /// <summary>
    /// Mostra il popup. 
    /// ORA ACCETTA UNA STRINGA 'costDescription' INVECE DI UN INT
    /// </summary>
    public void Show(PortalTeleporter portal, bool canUsePortal, string costDescription)
    {
        currentPortal = portal;

        // Troviamo il player per poterlo sbloccare se preme "Annulla"
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            playerMovementScript = playerObj.GetComponent<NewPlayerMovement>();
        }

        popupPanel.SetActive(true);

        if (canUsePortal)
        {
            // Usa direttamente la stringa passata dal portale (es. "Penalità: +30s")
            messageText.text = $"Do you want to use the portal? {costDescription}";
            usePortalButton.gameObject.SetActive(true);
        }
        else
        {
            messageText.text = $"Risorse insufficienti.\nServe: {costDescription}";
            usePortalButton.gameObject.SetActive(false);
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
