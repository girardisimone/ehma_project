using UnityEngine;
public class DrunkStrategy : IMovementStrategy
{
    public Vector2 CalculateMovement(Vector2 input, float baseSpeed)
    {
        // Se non c'è input, l'ubriaco barcolla comunque un po' da fermo? 
        // Facciamo che barcolla solo se si muove per non essere frustrante.
        if (input == Vector2.zero) return Vector2.zero;

        // Creiamo un disturbo basato sul tempo (Seno e Coseno a frequenze diverse)
        float swayX = Mathf.Sin(Time.time * 5f) * 0.5f; // Oscilla a destra/sinistra
        float swayY = Mathf.Cos(Time.time * 4f) * 0.5f; // Oscilla su/giù

        // Sommiamo il disturbo all'input originale
        Vector2 drunkInput = input + new Vector2(swayX, swayY);

        // Normalizziamo per evitare velocità eccessive in diagonale sballata
        // ma manteniamo un po' di irregolarità
        return drunkInput.normalized * baseSpeed;
    }
}