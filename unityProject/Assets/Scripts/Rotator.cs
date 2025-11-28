using UnityEngine;

public class Rotator : MonoBehaviour
{
    [Header("Impostazioni Rotazione")]
    [Tooltip("Velocità di rotazione in gradi al secondo. Usa valori negativi per girare in senso orario.")]
    public float rotationSpeed = 50f;

    void Update()
    {
        // Ruota costantemente l'oggetto sull'asse Z
        transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);
    }
}