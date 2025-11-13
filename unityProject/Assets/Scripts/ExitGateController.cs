using System.Collections.Generic;
using UnityEngine;

public class ExitGateController : MonoBehaviour
{
    [Header("Gate parts")]
    [SerializeField] private List<Collider2D> collidersToDisable; // solo i collider solidi
    [SerializeField] private GameObject closedVisual;
    [SerializeField] private GameObject openVisual;

    [Header("Popup UI")]
    [SerializeField] private ExitPopup exitPopup; // riferimento allo script ExitPopup

    private bool gateOpened = false;

    private void Start()
    {
        if (closedVisual) closedVisual.SetActive(true);
        if (openVisual) openVisual.SetActive(false);
        foreach (var c in collidersToDisable)
            if (c) c.enabled = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (gateOpened) return;
        if (!collision.CompareTag("Player")) return;

        if (exitPopup != null)
        {
            exitPopup.OnChooseExit.RemoveAllListeners();
            exitPopup.OnChooseContinue.RemoveAllListeners();

            exitPopup.OnChooseExit.AddListener(OpenGate);
            exitPopup.OnChooseContinue.AddListener(() =>
            {
                // non succede nulla, il popup si chiude da solo
            });

            exitPopup.Show();
        }
    }

    private void OpenGate()
    {
        gateOpened = true;

        foreach (var c in collidersToDisable)
            if (c) c.enabled = false;

        if (closedVisual) closedVisual.SetActive(false);
        if (openVisual) openVisual.SetActive(true);
    }
}