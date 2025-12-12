using UnityEngine;

public class PlayerIdentity : MonoBehaviour
{
    
    public enum HeroType { Topo, GoldenRacoon, CiclopeRosso, Gufo }

  
    public HeroType chiSono;

    void Start()
    {
        
        EnemySpawner regista = FindAnyObjectByType<EnemySpawner>();

        
        if (regista != null)
        {
            regista.AccendiNemiciGiusti(chiSono);
        }
    }
}