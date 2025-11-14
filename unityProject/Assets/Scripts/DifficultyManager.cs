using UnityEngine;
using System.Collections;
using UnityEngine.Rendering.Universal; // Importa il namespace per le luci (se usi URP)

public class DifficultyManager : MonoBehaviour
{
    // Singleton (per essere chiamato facilmente da PortalTeleporter)
    public static DifficultyManager Instance;

    // Variabili configurabili nell'Inspector
    [Header("Impostazioni Difficoltà")]
    [Tooltip("Numero di portali necessari per attivare l'effetto")]
    public int portalsToTriggerEffect = 3;
    [Tooltip("Durata dell'effetto in secondi")]
    public float effectDuration = 8f;

    [Header("Riferimenti Scena")]
    // Trascina il tuo Global Light 2D qui
    public Light2D globalLight;

    // Variabili di stato
    private int portalsUsedCount = 0;
    private float originalLightIntensity; // Per memorizzare la luce iniziale

    // Lo stato attuale dell'effetto (per evitare interferenze)
    private bool isEffectActive = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // Salva l'intensità iniziale della luce all'avvio
            if (globalLight != null)
            {
                originalLightIntensity = globalLight.intensity;
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Metodo chiamato dallo script PortalTeleporter
    public void RegisterPortalUse()
    {
        if (isEffectActive)
        {
            // Se l'effetto è già attivo, lo resettiamo per riavviare il timer
            StopAllCoroutines();
            StartCoroutine(DarknessEffectCoroutine());
            Debug.Log("Effetto oscurità resettato: il giocatore ha preso un altro portale.");
            return;
        }

        portalsUsedCount++;
        Debug.Log($"Portali usati: {portalsUsedCount} / {portalsToTriggerEffect}");

        // Controlla se abbiamo raggiunto la soglia
        if (portalsUsedCount >= portalsToTriggerEffect)
        {
            portalsUsedCount = 0; // Resetta il contatore
            StartCoroutine(DarknessEffectCoroutine());
        }
    }

    // Coroutine per gestire la durata temporizzata
    IEnumerator DarknessEffectCoroutine()
    {
        isEffectActive = true;

        // 1. ATTIVA L'EFFETTO BUIO: Riduci l'intensità della luce
        if (globalLight != null)
        {
            // Puoi anche impostare la sua intensità a 0.1 o un valore basso
            globalLight.intensity = 0.3f;
        }

        // 2. ATTENDI la durata specificata
        yield return new WaitForSeconds(effectDuration);

        // 3. SE NESSUN PORTALE E' STATO PRESO DURANTE L'ATTESA, DISATTIVA L'EFFETTO

        // Se non abbiamo preso un portale che ha resettato la Coroutine, torniamo normale
        // Torna all'intensità originale
        if (globalLight != null)
        {
            globalLight.intensity = originalLightIntensity;
        }

        isEffectActive = false;
        Debug.Log("L'effetto oscurità è terminato. Visione normale ripristinata.");
    }
}