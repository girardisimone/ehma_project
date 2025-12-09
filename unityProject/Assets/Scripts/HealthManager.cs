using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem; // <--- FONDAMENTALE: Aggiungi questa libreria

public class HealthManager : MonoBehaviour
{
    public static HealthManager Instance;
    
    public PortalManager portalManager;

    [Header("Configurazione")]
    [Tooltip("Numero di cuori da mostrare a schermo")]
    public int numberOfHearts = 3;

    public int currentHealth;
    public int maxHealth;

    [Header("Riferimenti UI")]
    public Image[] heartContainers;

    [Header("Sprites del Cuore")]
    public Sprite[] heartStates;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        maxHealth = numberOfHearts * 4;
        currentHealth = maxHealth;

        UpdateHealthUI();
    }

    // --- Funzioni di Debug Aggiornate (New Input System) ---
    void Update()
    {
        // Verifica che la tastiera sia collegata
        if (Keyboard.current == null) return;

        // Premi 'H' per farti male (Hurt)
        // Invece di Input.GetKeyDown(KeyCode.H) usiamo:
        if (Keyboard.current.hKey.wasPressedThisFrame)
        {
            TakeDamage(1);
        }

        // Premi 'R' per curarti (Restore)
        if (Keyboard.current.rKey.wasPressedThisFrame)
        {
            Heal(1);
        }
    }
    // -------------------------------------------------------

    public void TakeDamage(int damage)
    {
        // currentHealth -= damage;
        
        currentHealth -= 16; // da togliere
        if (currentHealth < 0) currentHealth = 0;

        UpdateHealthUI();
        
        if (currentHealth <= 2)
        {
            portalManager.CambiaProbabilitaToSamurai();
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(int amount)
    {
        currentHealth += amount;
        if (currentHealth > maxHealth) currentHealth = maxHealth;

        UpdateHealthUI();
    }

    void UpdateHealthUI()
    {
        for (int i = 0; i < heartContainers.Length; i++)
        {
            int heartStatus = currentHealth - (i * 4);
            int spriteIndex = Mathf.Clamp(heartStatus, 0, 4);

            if (spriteIndex < heartStates.Length)
            {
                heartContainers[i].sprite = heartStates[spriteIndex];
            }
        }
    }

    void Die()
    {
        Debug.Log("GAME OVER! Il giocatore è morto.");
    }
    
    // --- per l'interazione con il samurai ---
    public void RestoreMaxHealth()
    {
        currentHealth = maxHealth;
        UpdateHealthUI();
        Debug.Log("Salute completamente ripristinata!");
    }
}