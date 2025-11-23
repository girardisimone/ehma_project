using UnityEngine;
// Se usi Cinemachine, scommenta la riga sotto:
// using Unity.Cinemachine; 

public class CharacterSpawner : MonoBehaviour
{
    [Header("I Prefab dei Personaggi (Nello stesso ordine del Menu!)")]
    public GameObject[] characterPrefabs;

    [Header("Riferimenti da collegare")]
    public RoomCameraFollow roomCameraScript; // Il tuo script della telecamera
    // public CinemachineCamera cinemachineCam; // Se usi Cinemachine diretta

    private void Awake()
    {
        // 1. Leggi la scelta (Default 0 se non trova nulla)
        int index = PlayerPrefs.GetInt("SelectedCharacter", 0);

        // 2. Crea il personaggio
        GameObject spawnedPlayer = Instantiate(characterPrefabs[index], transform.position, Quaternion.identity);

        // 3. Rinominalo per evitare problemi con i Tag o ricerche per nome
        spawnedPlayer.name = "Player";

        // 4. AGGIORNA LA TELECAMERA (Fondamentale!)
        if (roomCameraScript != null)
        {
            roomCameraScript.target = spawnedPlayer.transform;
        }

        // Se usi Cinemachine standard, devi aggiornare il Follow:
        // if (cinemachineCam != null) cinemachineCam.Follow = spawnedPlayer.transform;
    }
}