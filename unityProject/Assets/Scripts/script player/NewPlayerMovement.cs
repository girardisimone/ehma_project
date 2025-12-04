using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(AudioSource))] // NUOVO: Aggiunge automaticamente l'AudioSource se manca
public class NewPlayerMovement : MonoBehaviour
{
    public float moveSpeed = 10f;
    private Rigidbody2D rb;
    private Vector2 moveInput;

    private Animator animator;

    // Variabili Strategia
    private IMovementStrategy movementStrategy;

    public bool isImmuneToMalus = false;
    public float malusDurationMultiplier = 1.0f;

    // --- 1. NUOVO: VARIABILI AUDIO ---
    [Header("Audio Passi")]
    public AudioClip grassSound; // Trascina qui il file audio
    [Range(0.1f, 1f)]
    public float stepRate = 0.4f; // Ogni quanto suona (più basso = passi più veloci)
    [Range(0f, 1f)]
    public float volumePassi = 0.5f; // Volume specifico dei passi

    private AudioSource audioSource;
    private float nextStepTime = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        // --- 2. NUOVO: PREPARIAMO L'AUDIO ---
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false; // Evita suoni strani all'avvio

        if (rb == null) Debug.LogError("Manca il Rigidbody2D!");
        if (animator == null) Debug.LogError("Manca l'Animator!");

        SetStrategy(new NormalMovementStrategy());
    }

    public void SetStrategy(IMovementStrategy newStrategy)
    {
        movementStrategy = newStrategy;
        Debug.Log($"Strategia di movimento cambiata in: {newStrategy.GetType().Name}");
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    // --- 3. NUOVO: USIAMO UPDATE PER GESTIRE IL SUONO ---
    // Usiamo Update (non FixedUpdate) perché l'audio non dipende dalla fisica
    void Update()
    {
        GestisciSuonoPassi();
    }

    void GestisciSuonoPassi()
    {
        // Controlliamo se ci stiamo muovendo davvero.
        // Usiamo rb.linearVelocity.magnitude invece di moveInput.
        // PERCHÉ? Se una strategia "Lag" ti blocca anche se premi i tasti, 
        // con questo controllo il suono dei passi si ferma (molto più realistico).
        bool siStaMuovendoFisicamente = rb.linearVelocity.magnitude > 0.1f;

        if (siStaMuovendoFisicamente && Time.time >= nextStepTime)
        {
            PlayFootstep();
            nextStepTime = Time.time + stepRate;
        }
    }

    void PlayFootstep()
    {
        if (grassSound == null) return;

        // Variazione del Pitch (Tono) per realismo
        audioSource.pitch = Random.Range(0.9f, 1.1f);
        // Variazione leggera del volume
        audioSource.volume = volumePassi * Random.Range(0.9f, 1.0f);

        audioSource.PlayOneShot(grassSound);
    }

    void FixedUpdate()
    {
        Vector2 finalVelocity = Vector2.zero;

        if (movementStrategy != null)
        {
            finalVelocity = movementStrategy.CalculateMovement(moveInput, moveSpeed);
        }
        else
        {
            finalVelocity = moveInput * moveSpeed;
        }

        // Applicazione fisica (Unity 6+)
        rb.linearVelocity = finalVelocity;

        if (animator != null)
        {
            animator.SetFloat("InputX", moveInput.x);
            animator.SetFloat("InputY", moveInput.y);
        }
    }
}