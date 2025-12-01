using UnityEngine;

public class PlayerIdentity : MonoBehaviour
{
    // L'elenco dei possibili eroi
    public enum HeroType { Topo, GoldenRacoon, CiclopeRosso, Gufo }

    // Qui selezionerai chi è questo specifico personaggio
    public HeroType chiSono;

    void Start()
    {
        // 1. Cerca il "Regista" (EnemySpawner) nella scena
        EnemySpawner regista = FindAnyObjectByType<EnemySpawner>();

        // 2. Se lo trova, gli dice: "Configura i nemici per ME!"
        if (regista != null)
        {
            regista.AccendiNemiciGiusti(chiSono);
        }
    }
}