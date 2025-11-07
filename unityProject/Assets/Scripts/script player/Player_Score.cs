using UnityEngine;

public class PlayerScore : MonoBehaviour
{
    // Rende il punteggio visibile nell'Inspector
    public int score = 0;

    // Metodo per aumentare il punteggio (chiamato dalla moneta)
    public void AddScore(int amount)
    {
        score += amount;
        // Questo apparirà nella Console:
        Debug.Log("Punteggio attuale: " + score);

        // In un gioco vero, qui aggiorneresti la UI!
    }
}