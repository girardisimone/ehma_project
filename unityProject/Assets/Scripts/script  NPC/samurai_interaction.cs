using UnityEngine;
using System.Collections;
using UnityEngine.UI; 

public class samurai_interaction : MonoBehaviour
{
    public static samurai_interaction Instance; // Per permettere ai portali di trovarlo

    [Header("Collegamenti UI")]
    public GameObject popupWindow;
    public GameObject messaggioSfidaUI; 

    [Header("Impostazioni Sfida")]
    public float tempoDiAstinenza = 10.0f; // Quanto tempo devi resistere senza portali

    // Riferimenti
    public HealthManager healthManager;
    public TimerManager timerManager;

    private bool sfidaInCorso = false;
    private Coroutine coroutineSfida;

    private void Awake()
    {
        // Crea un riferimento statico per essere trovato dai portali
        if (Instance == null) Instance = this;
    }

    private void Start()
    {
        if (popupWindow != null) popupWindow.SetActive(false);
        if (messaggioSfidaUI != null) messaggioSfidaUI.SetActive(false);

        healthManager = HealthManager.Instance;
       
    }

    // --- 1. CHIAMATO DAL BOTTONE "ACCETTA CONSIGLIO" ---
    public void AccettaConsiglio()
    {
        if (!sfidaInCorso)
        {
            // Chiude il dialogo
            if (popupWindow != null) popupWindow.SetActive(false);
            
            // Avvia il timer in background
            coroutineSfida = StartCoroutine(CountdownAstinenza());
        }
    }

    // --- 2. IL TIMER IN BACKGROUND ---
    private IEnumerator CountdownAstinenza()
    {
        sfidaInCorso = true;
        Debug.Log("SFIDA INIZIATA: Resisti senza portali per " + tempoDiAstinenza + " secondi!");
        
        if (messaggioSfidaUI != null) messaggioSfidaUI.SetActive(true);

        // Aspetta il tempo necessario (mentre il giocatore gioca)
        yield return new WaitForSeconds(tempoDiAstinenza);

        // --- SE ARRIVA QUI, HA VINTO! ---
        ApplicaCura();
    }

    // --- 3. SUCCESSO ---
    private void ApplicaCura()
    {
        sfidaInCorso = false;
        if (messaggioSfidaUI != null) messaggioSfidaUI.SetActive(false);

        Debug.Log("SFIDA SUPERATA! Il Samurai ti ha curato.");

        // Ripristina vite
        if (healthManager != null)
        {
            healthManager.RestoreMaxHealth();
        }
        else
        {
            Debug.Log("health manager null");
        }

        // Porta indietro il tempo
        if (timerManager != null)
        {
            timerManager.RemoveTime(60);
        }
        else
        {
            Debug.Log("time manager null");
        }
    }

    // --- 4. FALLIMENTO (Chiamato dai portali) ---
    public void GiocatoreHaUsatoPortale()
    {
        if (sfidaInCorso)
        {
            Debug.Log("SFIDA FALLITA: Hai ceduto alla tentazione del portale!");
            
            // Ferma il timer
            if (coroutineSfida != null) StopCoroutine(coroutineSfida);
            
            sfidaInCorso = false;
            if (messaggioSfidaUI != null) messaggioSfidaUI.SetActive(false);
        }
    }

    // --- GESTIONE TRIGGER (Apre solo il menu) ---
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (popupWindow != null) popupWindow.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (popupWindow != null) popupWindow.SetActive(false);
        }
    }
    
    public void Decline()
    {
        
        if (popupWindow != null) popupWindow.SetActive(false);
    }
}