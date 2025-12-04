using UnityEngine;

public class Gem : MonoBehaviour
{
    public AudioClip pickupSound; // Trascina il file audio qui nell'Inspector della Gemma

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // 1. Suona l'audio in un oggetto temporaneo
            if (pickupSound != null)
            {
                // PlayClipAtPoint crea un oggetto audio temporaneo alla posizione della gemma
                AudioSource.PlayClipAtPoint(pickupSound, transform.position);
            }

            // 2. Logica Punteggio (La tua logica esistente o richiamo al Manager)
            
            ScoreManager.GemCount++;
            if (ScoreManager.Instance != null)
                ScoreManager.Instance.UpdateScoreText(ScoreManager.GemCount);

            // 3. Distruggi la gemma
            Destroy(gameObject);
        }
    }
}