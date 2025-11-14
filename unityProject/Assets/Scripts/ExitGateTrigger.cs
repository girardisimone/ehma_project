using UnityEngine;

public class ExitGateTrigger : MonoBehaviour
{
    [Header("Popup da attivare")]
    public GameObject exitPopup;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Il player ha raggiunto l'uscita.");
            if (exitPopup != null)
            {
                exitPopup.SetActive(true);
            }
        }
    }
}
    