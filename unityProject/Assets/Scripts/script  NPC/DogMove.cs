using UnityEngine;

public class DogCompanion : MonoBehaviour
{
    [Header("Impostazioni Pattuglia")]
    public Transform puntoA;
    public Transform puntoB;
    public float patrolSpeed = 2f;

    [Header("Impostazioni Seguimi")]
    private Transform player;
    public float followSpeed = 3f;
    public float stopDistance = 1.5f;

    [Header("Interfaccia")]
    public GameObject popupDialogo;

    private Transform targetPattuglia;
    private bool inDialogo = false;
    private bool isFollowing = false;
    private bool isWaiting = false;

    void Start()
    {
        transform.parent = null;
        targetPattuglia = puntoB;

        if (popupDialogo != null) popupDialogo.SetActive(false);
    }

    void Update()
    {
        // --- RICERCA AUTOMATICA DEL PLAYER ---
        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null)
            {
                player = p.transform;
            }
            else
            {
                return;
            }
        }

        // --- LOGICA NORMALE ---
        if (isFollowing)
        {
            SeguiIlPlayer();
        }
        else if (isWaiting)
        {
            // STA FERMO
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
        if (other.CompareTag("Player") && !isFollowing && !inDialogo)
        {
            AttivaDialogo();
        }
    }

    // --- NUOVA AGGIUNTA QUI SOTTO ---
    private void OnTriggerExit2D(Collider2D other)
    {
        // Se il player si allontana e stavamo parlando (ma non seguendo)
        if (other.CompareTag("Player") && inDialogo && !isFollowing)
        {
            if (popupDialogo != null) popupDialogo.SetActive(false);
            inDialogo = false; // Questo farà ripartire la pattuglia nel prossimo Update
        }
    }

    public void RestaQui()
    {
        isFollowing = false;
        isWaiting = true;
    }

    void AttivaDialogo()
    {
        isWaiting = false; 
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