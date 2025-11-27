using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public enum PlayerArchetype
{
	Normal,
    Drunk,
    Drugged,
    InternetAddict,
    Gambler
}

public class NewPlayerMovement : MonoBehaviour
{

    public float moveSpeed = 10f;
	[Header("Archetipo (Il vizio nascosto)")]
    // Seleziona qui dall'Inspector che tipo di giocatore è questo (Ubriaco, Gambler, ecc.)
    
    [Header("Immunità ai malus dei portali")]
    public bool isImmuneToMalus = false; // Se vero, i portali non fanno nulla
    public float malusDurationMultiplier = 1.0f; // 1 = normale, 2 = durata doppia
    public PlayerArchetype initialArchetype = PlayerArchetype.Normal;

    private Rigidbody2D rb;
    private Vector2 moveInput;
    private Animator animator;
	private IMovementStrategy currentMovementStrategy;

	// Logica Strategia movimento
    private IMovementStrategy currentStrategy;
    private IMovementStrategy normalStrategy;   // Strategia di default (camminata sana)
    private IMovementStrategy penaltyStrategy;  // Strategia del vizio (attivata dai portali)

    // Gestione Timer Penalità
    private Coroutine penaltyCoroutine;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
		if (rb == null) Debug.LogError("Manca il Rigidbody2D!");

        animator = GetComponent<Animator>();
       	if (animator == null) Debug.LogError("Manca l'Animator!");

		// 1. Creiamo le istanze delle strategie
        normalStrategy = new NormalMovementStrategy();
        penaltyStrategy = CreateStrategyFromArchetype(initialArchetype);

        // 2. Partiamo SEMPRE con il movimento normale
        SetStrategy(normalStrategy);
    }

	// Factory semplice per creare la classe giusta in base all'Enum
    private IMovementStrategy CreateStrategyFromArchetype(PlayerArchetype type)
    {
        switch (type)
        {
            case PlayerArchetype.Drunk: 
                return new DrunkSluggishStrategy();
            case PlayerArchetype.Drugged: 
                return new DruggedStrategy();
            case PlayerArchetype.InternetAddict: 
                return new InternetPacketLossStrategy();
            case PlayerArchetype.Gambler: 
                return new GamblerRouletteStrategy();
            default: 
                return new NormalMovementStrategy();
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

 	void FixedUpdate()
    {
        // 1. Calcolo Movimento (Delega alla strategia corrente)
        Vector2 finalVelocity = currentStrategy != null 
            ? currentStrategy.CalculateMovement(moveInput, moveSpeed) 
            : Vector2.zero;

        rb.linearVelocity = finalVelocity; // Usa .velocity se sei su Unity versioni vecchie

        // 2. Gestione Animazioni
        if (animator != null)
        {
            // Se c'è una penalità attiva, l'animazione segue la velocità reale per coerenza visiva
            // Altrimenti segue l'input del giocatore per reattività
            bool isPenaltyActive = currentStrategy == penaltyStrategy;
            
            Vector2 animDir = (isPenaltyActive && finalVelocity.sqrMagnitude > 0.1f) 
                ? finalVelocity.normalized 
                : moveInput;

            animator.SetFloat("InputX", animDir.x);
            animator.SetFloat("InputY", animDir.y);
        }
    }

	
     
    private IEnumerator PenaltyRoutine(float duration)
    {
        // Attiva il movimento "Pazzo"
        SetStrategy(penaltyStrategy);
        Debug.Log($"[Player] Penalità attivata: {initialArchetype} per {duration} secondi.");

        yield return new WaitForSeconds(duration);

        // Torna al movimento "Normale"
        SetStrategy(normalStrategy);
        Debug.Log("[Player] Penalità terminata. Movimento normale.");
        
        penaltyCoroutine = null;
    }

    private void SetStrategy(IMovementStrategy newStrategy)
    {
        currentStrategy = newStrategy;
    }
    
    // --- CHIAMATO DAL DIFFICULTY MANAGER ---
    public void ActivatePenaltyEffect(float baseDuration)
    {
        // 1. Controllo Immunità
        if (isImmuneToMalus)
        {
            Debug.Log("IMMUNE! Il polpo ti protegge, il portale non ha effetto.");
            return; // Esce dalla funzione, niente malus!
        }

        // 2. Calcolo durata effettiva (se ho appena usato l'immunità il malus durerà di più del "normale")
        float effectiveDuration = baseDuration * malusDurationMultiplier;

        // RESET: Se c'è già un timer in corso, lo fermiamo.
        if (penaltyCoroutine != null)
        {
            StopCoroutine(penaltyCoroutine);
        }

        // Avviamo il timer con la durata calcolata
        penaltyCoroutine = StartCoroutine(PenaltyRoutine(effectiveDuration));
    }
}