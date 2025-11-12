using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Rimuovi la variabile 'scoreManager' e il metodo Awake()/Start()

    // La variabile GemCount è stata spostata in ScoreManager.cs

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Gem"))
        {
            Destroy(collision.gameObject);

            // 1. Incrementa il conteggio STATICO
            ScoreManager.GemCount++;

            // 2. Aggiorna la UI usando l'istanza SINGOLA
            if (ScoreManager.Instance != null)
            {
                ScoreManager.Instance.UpdateScoreText(ScoreManager.GemCount);
            }
        }
    }
}