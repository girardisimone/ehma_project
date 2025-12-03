using UnityEngine;

public class SimplePathTrigger : MonoBehaviour
{
    // Qui trascinerai l'oggetto "SentieroMagico" che contiene tutte le stelle
    public GameObject pathContainer;

    // Questa funzione la collegheremo al bottone "OK"
    public void ShowThePath()
    {
        if (pathContainer != null)
        {
            pathContainer.SetActive(true); // ACCENDE il sentiero
            Debug.Log("Sentiero attivato!");
        }
        else
        {
            Debug.LogError("Hai dimenticato di collegare l'oggetto SentieroMagico nello script!");
        }
    }
}

