using UnityEngine;

public class DogCompanion : MonoBehaviour
{
    [Header("Impostazioni Pattuglia")]
    public Transform puntoA;
    public Transform puntoB;
    public float patrolSpeed = 2f;

    [Header("Impostazioni Seguimi")]
    // NON è più pubblica, se lo cerca da solo!
    private Transform player;
    public float followSpeed = 3f;
    public float stopDistance = 1.5f;

    [Header("Interfaccia")]
    public GameObject popupDialogo;

    private Transform targetPattuglia;
    private bool inDialogo = false;
    private bool isFollowing = false;
    private bool isWaiting = false; // NUOVA VARIABILE: Il cane è in attesa?

    void Start()
    {
        transform.parent = null;
        targetPattuglia = puntoB;

        if (popupDialogo != null) popupDialogo.SetActive(false);
    }

    void Update()
    {
        // --- RICERCA AUTOMATICA DEL PLAYER ---
        // Se non sappiamo chi è il player (perché non è ancora spawnato)...
        if (player == null)
        {
            // ...cerchiamo un oggetto con il tag "Player"
            GameObject p = GameObject.FindGameObjectWithTag("Player");

            // Se lo troviamo, lo memorizziamo!
            if (p != null)
            {
                player = p.transform;
            }
            else
            {
                // Se non c'è ancora il player, il cane aspetta e non fa nulla
                return;
            }
        }

        // --- LOGICA NORMALE ---
        if(isFollowing)
    {
            SeguiIlPlayer();
        }
    else if (isWaiting)
        {
            // STA FERMO: Non fa nulla, non pattuglia, aspetta solo te.
            // Puoi aggiungere qui un'animazione "Idle" o "Seduto" se ce l'hai.
        }
        else if (!inDialogo)
        {
            Pattuglia();
        }
    }

    void Pattuglia()
    {
        transform.position = Vector2.MoveTowards(transform.position, targetPattuglia.position, patrolSpeed * Time.deltaTime);

        if (Vector2.Distance(transform.position, targetPattuglia.position) < 0.1f)
        {
            if (targetPattuglia == puntoB)
            {
                targetPattuglia = puntoA;
                Girati(-1);
            }
            else
            {
                targetPattuglia = puntoB;
                Girati(1);
            }
        }
    }

    void SeguiIlPlayer()
    {
        // Sicurezza extra: se il player muore o sparisce, smetti di seguire
        if (player == null) return;

        float distanza = Vector2.Distance(transform.position, player.position);

        if (distanza > stopDistance)
        {
            transform.position = Vector2.MoveTowards(transform.position, player.position, followSpeed * Time.deltaTime);
        }

        if (player.position.x > transform.position.x)
        {
            Girati(1);
        }
        else
        {
            Girati(-1);
        }
    }

    void Girati(int direzioneX)
    {
        transform.localScale = new Vector3(direzioneX, 1, 1);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Verifica che chi tocca sia VERAMENTE il player (grazie al Tag)
        if (other.CompareTag("Player") && !isFollowing && !inDialogo)
        {
            AttivaDialogo();
        }
    }
    // --- NUOVA FUNZIONE PUBBLICA DA CHIAMARE DAL PORTALE ---
    public void RestaQui()
    {
        isFollowing = false; // Smette di seguire
        isWaiting = true;    // Entra in modalità statua
    }

    // --- MODIFICA QUESTA FUNZIONE ESISTENTE ---
    void AttivaDialogo()
    {
        isWaiting = false; // Se lo tocchi di nuovo, smette di aspettare e ti ascolta
        inDialogo = true;
        if (popupDialogo != null) popupDialogo.SetActive(true);
    }

    public void FineDialogo()
    {
        if (popupDialogo != null) popupDialogo.SetActive(false);
        inDialogo = false;
        isFollowing = true;
    }
}