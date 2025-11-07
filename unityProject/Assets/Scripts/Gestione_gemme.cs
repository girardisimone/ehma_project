using UnityEngine;

public class Coin : MonoBehaviour
{
    public int value = 1; // Valore della moneta (1 punto)

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Controlla se l'oggetto che ha toccato il trigger è il giocatore
        if (other.CompareTag("Player"))
        {
            // Tenta di ottenere il componente PlayerScore dal giocatore
            PlayerScore player = other.GetComponent<PlayerScore>();

            if (player != null)
            {
                // 1. Aggiunge il punteggio
                player.AddScore(value);

                // 2. Distrugge la moneta dall'Hierarchy
                Destroy(gameObject);
            }
        }
    }
}
