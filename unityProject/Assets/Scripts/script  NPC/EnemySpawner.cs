using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Trascina qui i nemici dalla scena")]
    public GameObject octopus; // Internet
    public GameObject maiale;  // Alcol/Droga
    public GameObject sultano; // Gambling

    // Questa funzione viene chiamata dal Player appena entra
    public void AccendiNemiciGiusti(PlayerIdentity.HeroType eroe)
    {
        // DEBUG: Vediamo chi sta parlando!
        Debug.Log("ATTENZIONE! L'ordine è arrivato da questo oggetto: " + eroe);

        // Spegniamo tutto
        if (octopus != null) octopus.SetActive(false);
        if (maiale != null) maiale.SetActive(false);
        if (sultano != null) sultano.SetActive(false);

        // Riaccendiamo
        switch (eroe)
        {
            case PlayerIdentity.HeroType.Topo:
                if (maiale != null) maiale.SetActive(true);
                if (octopus != null) octopus.SetActive(true);
                break;

            case PlayerIdentity.HeroType.GoldenRacoon:
                if (sultano != null) sultano.SetActive(true);
                if (maiale != null) maiale.SetActive(true);
                break;

            // ... (gli altri case rimangono uguali)
            case PlayerIdentity.HeroType.CiclopeRosso:
                if (octopus != null) octopus.SetActive(true);
                break;

            case PlayerIdentity.HeroType.Gufo:
                if (octopus != null) octopus.SetActive(true);
                if (maiale != null) maiale.SetActive(true);
                if (sultano != null) sultano.SetActive(true);
                break;
        }
    }
    }