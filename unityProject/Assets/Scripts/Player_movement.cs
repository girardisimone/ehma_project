using UnityEngine;
using UnityEngine.InputSystem; // Importante: devi importare la libreria del nuovo sistema

public class NewPlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    private Rigidbody2D rb;
    private Vector2 moveInput;

    void Start()
    {
        // Ottiene il riferimento al Rigidbody (la fisica)
        rb = GetComponent<Rigidbody2D>();

        // Assicurati che l'oggetto abbia il Rigidbody e il Collider 2D
        if (rb == null || GetComponent<Collider2D>() == null)
        {
            Debug.LogError("Il giocatore necessita di Rigidbody2D e Collider2D!");
        }
    }

    // Metodo chiamato automaticamente da Player Input quando l'azione 'Move' cambia.
    // Il nome di questo metodo deve corrispondere al nome dell'azione (OnMove).
    public void OnMove(InputAction.CallbackContext context)
    {
        // Legge il valore Vector2 dall'azione
        moveInput = context.ReadValue<Vector2>();
    }

    void FixedUpdate()
    {
        // Applica il movimento basato sulla fisica
        Vector2 finalVelocity = moveInput * moveSpeed;
        rb.linearVelocity = finalVelocity;
    }
}