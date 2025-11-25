using UnityEngine;
using System.Collections;
using UnityEngine.Rendering.Universal; // Necessario per Light2D

public class DifficultyManager : MonoBehaviour
{
    public static DifficultyManager Instance;

    [Header("--- Difficoltà Movimento (Penalità) ---")]
    [Tooltip("Durata base della penalità al primo portale (secondi).")]
    public float basePenaltyDuration = 5f;

    [Tooltip("Quanti secondi si aggiungono alla durata per OGNI portale preso.")]
    public float penaltyIncrementPerPortal = 5f;

    // Livello attuale di difficoltà (quanti portali ho preso)
    private int penaltyLevel = 0;

    [Header("--- Difficoltà Visiva (Buio) ---")]
    [Tooltip("Ogni quanti portali scatta l'effetto buio?")]
    public int portalsToTriggerDarkness = 3;
    public float darknessDuration = 8f;
    public Light2D globalLight;

    private int portalsForDarknessCount = 0;
    private float originalLightIntensity;
    private bool isDarknessActive = false;

    private void Awake()
    {
        // Singleton Pattern
        if (Instance == null)
        {
            Instance = this;
            if (globalLight != null) originalLightIntensity = globalLight.intensity;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Chiamato da PortalTeleporter quando il player si teletrasporta.
    /// </summary>
    public void RegisterPortalUse()
    {
        // 1. GESTIONE MOVIMENTO 
        ApplyMovementPenalty();

        // 2. GESTIONE LUCI 
        HandleDarknessLogic();
    }

    private void ApplyMovementPenalty()
    {
        // Calcolo durata: Base + (Livello * Incremento)
        // Es: Base 5, Inc 2.
        // Portale 1 (lvl 0): 5s
        // Portale 2 (lvl 1): 7s
        // Portale 3 (lvl 2): 9s
        float currentDuration = basePenaltyDuration + (penaltyLevel * penaltyIncrementPerPortal);

        // Trova il player e applica l'effetto
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            NewPlayerMovement movementScript = player.GetComponent<NewPlayerMovement>();
            if (movementScript != null)
            {
                movementScript.ActivatePenaltyEffect(currentDuration);
            }
        }

        // Aumenta il livello di difficoltà per la prossima volta
        penaltyLevel++;
    }

    private void HandleDarknessLogic()
    {
        // Se è già buio, resetta il timer del buio
        if (isDarknessActive)
        {
            StopCoroutine("DarknessEffectCoroutine");
            StartCoroutine("DarknessEffectCoroutine");
            return;
        }

        portalsForDarknessCount++;
        
        // Se abbiamo raggiunto la soglia, attiva il buio
        if (portalsForDarknessCount >= portalsToTriggerDarkness)
        {
            portalsForDarknessCount = 0;
            StartCoroutine("DarknessEffectCoroutine");
        }
    }

    IEnumerator DarknessEffectCoroutine()
    {
        isDarknessActive = true;
        if (globalLight != null) globalLight.intensity = 0.3f; // Luci basse

        yield return new WaitForSeconds(darknessDuration);

        if (globalLight != null) globalLight.intensity = originalLightIntensity; // Luci normali
        isDarknessActive = false;
    }
}