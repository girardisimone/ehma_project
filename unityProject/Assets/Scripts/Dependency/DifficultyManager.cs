using UnityEngine;
using System.Collections;
using UnityEngine.Rendering.Universal;

public class DifficultyManager : MonoBehaviour
{
    public static DifficultyManager Instance;

    [Header("Impostazioni Difficoltà")]
    public int portalsToTriggerEffect = 3;
    public float effectDuration = 8f;

    [Header("Riferimenti Scena")]
    public Light2D globalLight;

    private int portalsUsedCount = 0;
    private float originalLightIntensity;
    private bool isEffectActive = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        if (globalLight != null) originalLightIntensity = globalLight.intensity;
    }

    public void RegisterPortalUse()
    {
        // Gestisce SOLO il conteggio per l'effetto visivo (BUIO)
        if (isEffectActive)
        {
            StopAllCoroutines();
            StartCoroutine(DarknessEffectCoroutine());
            return;
        }

        portalsUsedCount++;

        if (portalsUsedCount >= portalsToTriggerEffect)
        {
            portalsUsedCount = 0;
            StartCoroutine(DarknessEffectCoroutine());
        }
    }

    IEnumerator DarknessEffectCoroutine()
    {
        isEffectActive = true;

        // ATTIVA BUIO
        if (globalLight != null) globalLight.intensity = 0.3f;

        yield return new WaitForSeconds(effectDuration);

        // RIPRISTINA LUCE
        if (globalLight != null) globalLight.intensity = originalLightIntensity;

        isEffectActive = false;
    }
}