using UnityEngine;
using System.Collections;

public class Pig_interaction : MonoBehaviour
{
    [Header("Collegamenti UI")]
    public GameObject popupWindow;

    [Header("Impostazioni Pozione")]
    public float boostDuration = 5f;
    public float penaltyDuration = 3f;
    public float boostMultiplier = 2.0f;
    public float penaltyMultiplier = 0.5f;

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
    public void AcceptPotion()
    {
        // Chiudiamo il popup
        if (popupWindow != null) popupWindow.SetActive(false);

        // Se abbiamo un player valido, avviamo la magia
        if (currentPlayer != null)
        {
            StartCoroutine(PotionRoutine(currentPlayer));
        }
    }

	// codice chimato se il giocatore clicca NO sul popup
    public void DeclinePotion()
    {
        // Chiudiamo semplicemente il popup
        if (popupWindow != null) popupWindow.SetActive(false);
    }

    // --- LOGICA DELLA POZIONE ---
    private IEnumerator PotionRoutine(NewPlayerMovement player)
    {
        float originalSpeed = player.moveSpeed;

        // FASE 1: Veloce
        player.moveSpeed = originalSpeed * boostMultiplier;
        Debug.Log("Boost Attivato!");
        yield return new WaitForSeconds(boostDuration);

        // FASE 2: Lento
        player.moveSpeed = originalSpeed * penaltyMultiplier;
        Debug.Log("Malus Attivato!");
        yield return new WaitForSeconds(penaltyDuration);

        // FASE 3: Normale
        player.moveSpeed = originalSpeed;
        Debug.Log("Velocità Ripristinata.");
    }
}