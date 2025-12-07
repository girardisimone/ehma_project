using UnityEngine;
using System.Collections;
using UnityEngine.Rendering; // Per il Volume
using UnityEngine.Rendering.Universal; // Per URP (Luci e Post-processing)

public class DifficultyManager : MonoBehaviour
{
    public static DifficultyManager Instance;

    [Header("Impostazioni Difficoltà (Progressiva)")]
    public int portalsToTriggerEffect = 3;
    public float effectDuration = 8f;

    [Header("Riferimenti Scena")]
    public Light2D globalLight;
    public Volume globalVolume;

    // Variabili interne effetti
    private ChromaticAberration chromaticAberration;
    private LensDistortion lensDistortion;

    private int portalsUsedCount = 0;
    private float originalLightIntensity = 1f;
    private bool isEffectActive = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        // Salvataggio Luce
        if (globalLight != null)
        {
            originalLightIntensity = globalLight.intensity;
        }
        else
        {
            Debug.LogWarning("[DifficultyManager] Manca la Global Light nell'Inspector!");
        }

        // Recupero Effetti dal Volume
        if (globalVolume != null && globalVolume.profile != null)
        {
            globalVolume.profile.TryGet(out chromaticAberration);
            globalVolume.profile.TryGet(out lensDistortion);
        }
        else
        {
            Debug.LogWarning("[DifficultyManager] Manca il Global Volume nell'Inspector!");
        }
    }

    // --- 1. BUIO (Generico) ---
    public void ForceDarkness(float duration)
    {
        if (isEffectActive) StopAllCoroutines();
        StartCoroutine(DarknessCoroutine(duration));
    }

    IEnumerator DarknessCoroutine(float duration)
    {
        isEffectActive = true;

        if (globalLight != null) globalLight.intensity = 0.3f; // Buio

        yield return new WaitForSeconds(duration);

        if (globalLight != null) globalLight.intensity = originalLightIntensity; // Luce

        isEffectActive = false;
    }

    // --- 2. GLITCH (Internet) ---
    public void ForceGlitch(float duration)
    {
        StartCoroutine(GlitchCoroutine(duration));
    }

    IEnumerator GlitchCoroutine(float duration)
    {
        float timer = 0;

        while (timer < duration)
        {
            if (chromaticAberration != null)
                chromaticAberration.intensity.value = Random.Range(0.5f, 1f);

            if (lensDistortion != null)
                lensDistortion.intensity.value = Random.Range(-0.2f, 0.2f);

            yield return new WaitForSeconds(Random.Range(0.05f, 0.15f));
            timer += 0.1f;
        }

        // Reset finale
        if (chromaticAberration != null) chromaticAberration.intensity.value = 0f;
        if (lensDistortion != null) lensDistortion.intensity.value = 0f;
    }

    // --- 3. CONTEGGIO PORTALI ---
    public void RegisterPortalUse()
    {
        if (isEffectActive)
        {
            StopAllCoroutines();
            StartCoroutine(DarknessCoroutine(effectDuration));
            return;
        }

        portalsUsedCount++;

        if (portalsUsedCount >= portalsToTriggerEffect)
        {
            portalsUsedCount = 0;
            StartCoroutine(DarknessCoroutine(effectDuration));
        }
    }
}