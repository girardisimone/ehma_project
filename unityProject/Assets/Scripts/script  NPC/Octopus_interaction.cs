using UnityEngine;
using System.Collections;

public class Octopus_interaction : MonoBehaviour
{
    [Header("Collegamenti UI")]
    public GameObject popupWindow;

    [Header("Impostazioni immunità")]
    public float immunityDuration = 20f; // Tempo in cui sei immune ai portali
    public float penaltyDuration = 40f; // Tempo in cui i malus durano di più
    public float penaltyMultiplier = 2.0f; // I malus durano il DOPPIO (2x)

    // Riferimento interno al player corrente
    private NewPlayerMovement currentPlayer;

    private void Start()
    {
        if (popupWindow != null) popupWindow.SetActive(false);
    }

    // --- GESTIONE TRIGGER ---
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            currentPlayer = other.GetComponent<NewPlayerMovement>();
            if (popupWindow != null) popupWindow.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (popupWindow != null) popupWindow.SetActive(false);
            // currentPlayer = null; // Facoltativo: resetta il player se esce
        }
    }

    // codice chimato se il giocatore clicca SI sul popup
    public void AcceptImmunity()
    {
        // Chiudiamo il popup
        if (popupWindow != null) popupWindow.SetActive(false);

        // Se abbiamo un player valido, avviamo la magia
        if (currentPlayer != null)
        {
            StartCoroutine(ImmunityRoutine(currentPlayer));
        }
    }


	// codice chimato se il giocatore clicca NO sul popup
    public void DeclineImmunity()
    {
        // Chiudiamo semplicemente il popup
        if (popupWindow != null) popupWindow.SetActive(false);
    }

    // --- LA LOGICA DELL'IMMUNITA' ---
    private IEnumerator ImmunityRoutine(NewPlayerMovement player)
    {
        // FASE 1:  Immunità
        player.isImmuneToMalus = true;
        player.malusDurationMultiplier = 1.0f;
        Debug.Log("IMMUNITY: Sei immune ai portali!");

        yield return new WaitForSeconds(immunityDuration);

        // FASE 2: Malus prolungati
        player.isImmuneToMalus = false;
        player.malusDurationMultiplier = penaltyMultiplier; // Es. x2
        Debug.Log("IMMUNITY: Finito l'effetto dell'immunità... ora i malus dei portali dureranno di più!");

        yield return new WaitForSeconds(penaltyDuration);

        // FASE 3: NORMALITÀ
        player.malusDurationMultiplier = 1.0f;
        Debug.Log("IMMUNITY: effetto terminato");
    }
}