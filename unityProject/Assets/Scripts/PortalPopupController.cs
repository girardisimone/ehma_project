using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PortalPopupController : MonoBehaviour
{
    public static PortalPopupController Instance;

    [Header("Riferimenti UI")]
    public GameObject popupPanel;        // Il pannello del popup
    public TextMeshProUGUI messageText;  // Testo del messaggio
    public Button usePortalButton;       // Pulsante "Accedi al portale"
    public Button continueButton;        // Pulsante "Continua"

    private PortalTeleporter currentPortal;
    private int currentCost;

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
    /// Mostra il popup per un certo portale.
    /// </summary>
    public void Show(PortalTeleporter portal, bool canUsePortal, int gemCost)
    {
        currentPortal = portal;
        currentCost = gemCost;

        popupPanel.SetActive(true);

        if (canUsePortal)
        {
            messageText.text = $"Vuoi accedere al portale e lasciare {gemCost} gemme?";
            usePortalButton.gameObject.SetActive(true);
        }
        else
        {
            messageText.text = $"Per accedere ai portali ti servono almeno {gemCost} gemme.";
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
        Hide();
    }

    public void Hide()
    {
        popupPanel.SetActive(false);
        currentPortal = null;
    }

    /// <summary>
    /// Serve per chiudere il popup se il player esce dal trigger del portale.
    /// </summary>
    public void HideIfCurrent(PortalTeleporter portal)
    {
        if (currentPortal == portal)
        {
            Hide();
        }
    }
}
