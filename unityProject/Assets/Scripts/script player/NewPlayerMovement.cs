using UnityEngine;
using UnityEngine.InputSystem;

public class NewPlayerMovement : MonoBehaviour
{
    public float moveSpeed = 10f;
    private Rigidbody2D rb;
    private Vector2 moveInput;

    private Animator animator;

    // 1. NUOVO: La variabile che contiene la strategia attuale
    private IMovementStrategy movementStrategy;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        if (rb == null) Debug.LogError("Manca il Rigidbody2D!");
        if (animator == null) Debug.LogError("Manca l'Animator!");

        // 2. DEFAULT: Iniziamo con movimento normale
        SetStrategy(new NormalMovementStrategy());
    }

    // 3. METODO PUBBLICO: Permette ad altri script (DependencyManager) di cambiare la strategia
    public void SetStrategy(IMovementStrategy newStrategy)
    {
        movementStrategy = newStrategy;
        Debug.Log($"Strategia di movimento cambiata in: {newStrategy.GetType().Name}");
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    void FixedUpdate()
    {
        // 4. CALCOLO: Invece di fare il calcolo qui, chiediamo alla strategia
        Vector2 finalVelocity = Vector2.zero;

        if (movementStrategy != null)
        {
            finalVelocity = movementStrategy.CalculateMovement(moveInput, moveSpeed);
        }
        else
        {
            // Fallback di sicurezza
            finalVelocity = moveInput * moveSpeed;
        }

        // Applicazione fisica
        rb.linearVelocity = finalVelocity; // Nota: Unity 6 usa linearVelocity, versioni vecchie velocity

        // 5. ANIMAZIONE: Passiamo l'input "sporco" o quello reale?
        // Solitamente per l'animazione va bene l'input grezzo (moveInput) così le gambe si muovono
        // anche se il personaggio è bloccato dal lag, dando l'idea di sforzo.
        if (animator != null)
        {
            animator.SetFloat("InputX", moveInput.x);
            animator.SetFloat("InputY", moveInput.y);
        }
    }
}