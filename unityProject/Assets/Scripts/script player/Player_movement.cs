using UnityEngine;
using UnityEngine.InputSystem;

public class NewPlayerMovement : MonoBehaviour
{
    public float moveSpeed = 10f;
    private Rigidbody2D rb;
    private Vector2 moveInput;

    private Animator animator;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        if (rb == null) Debug.LogError("Manca il Rigidbody2D!");
        if (animator == null) Debug.LogError("Manca l'Animator!");
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    void FixedUpdate()
    {
        // 1. Movimento Fisico
        Vector2 finalVelocity = moveInput * moveSpeed;
        rb.linearVelocity = finalVelocity;

        // 2. Gestione Animazione (2D Blend Tree)
        if (animator != null)
        {
            // Passiamo InputX e InputY all'Animator
            animator.SetFloat("InputX", moveInput.x);
            animator.SetFloat("InputY", moveInput.y);
        }
    }
}