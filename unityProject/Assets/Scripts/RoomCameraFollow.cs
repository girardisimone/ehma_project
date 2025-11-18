using UnityEngine;

public class RoomCameraFollow : MonoBehaviour
{
    // Riferimenti da impostare nell'Inspector
    public Transform target;           // Trascina qui l'oggetto Player
    public float roomWidth = 25f;      // La larghezza esatta (in unità) di una singola griglia
    public float roomHeight = 25f;     // L'altezza esatta (in unità) di una singola griglia

    // Lo smorzamento per l'effetto di "scatto" (lascia 0)
    public float transitionSpeed = 0f;

    // Variabile per la posizione calcolata
    private Vector3 targetPosition;

    void LateUpdate()
    {
        if (target == null) return;

        // 1. Calcola l'indice della stanza in cui si trova il giocatore (Room Index)
        // La posizione del giocatore divisa per la dimensione della stanza (con arrotondamento all'intero più vicino)
        float targetX = target.position.x;
        float targetY = target.position.y;

        int roomX = Mathf.RoundToInt(targetX / roomWidth);
        int roomY = Mathf.RoundToInt(targetY / roomHeight);

        // 2. Calcola la posizione centrale esatta di quella stanza
        float cameraX = roomX * roomWidth; ;
        float cameraY = roomY * roomHeight;

        // 3. Imposta la posizione di destinazione (con la Z della telecamera)
        targetPosition = new Vector3(cameraX, cameraY, transform.position.z);

        // 4. Muove la telecamera (istantaneamente se transitionSpeed = 0)
        if (transitionSpeed <= 0)
        {
            // Movimento ISTANTANEO (scatto secco)
            transform.position = targetPosition;
        }
        else
        {
            // Movimento Smorzato (transizione fluida tra stanze)
            transform.position = Vector3.Lerp(transform.position, targetPosition, transitionSpeed * Time.deltaTime);
        }
    }
}
