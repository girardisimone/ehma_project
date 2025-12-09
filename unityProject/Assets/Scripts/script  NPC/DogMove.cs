using UnityEngine;
using System.Collections;

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
    public GameObject popupAbbandono;

    private Transform targetPattuglia;
    private bool inDialogo = false;
    private bool isFollowing = false;
    private bool isWaiting = false;

    // Timer per ignorare il player dopo aver detto NO
    private float timerIgnoraPlayer = 0f;

    void Start()
    {
        transform.parent = null;
        targetPattuglia = puntoB;

        if (popupDialogo != null) popupDialogo.SetActive(false);
        if (popupAbbandono != null) popupAbbandono.SetActive(false);
    }

    void Update()
    {
        // 1. GESTIONE TIMER: Se è maggiore di 0, scende
        if (timerIgnoraPlayer > 0)
        {
            timerIgnoraPlayer -= Time.deltaTime;
        }

        // --- RICERCA AUTOMATICA DEL PLAYER ---
        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) player = p.transform;
            else return;
        }

        // --- LOGICA MOVIMENTO ---
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

        if (player.position.x > transform.position.x) Girati(1);
        else Girati(-1);
    }

    void Girati(int direzioneX)
    {
        transform.localScale = new Vector3(direzioneX, 1, 1);
    }

    // --- COLLISIONI ---

    private void OnTriggerEnter2D(Collider2D other)
    {
        // MODIFICA IMPORTANTE: Aggiunto && timerIgnoraPlayer <= 0
        // Se il timer è attivo (ho appena detto no), NON apre il dialogo
        if (other.CompareTag("Player") && !isFollowing && !inDialogo && timerIgnoraPlayer <= 0)
        {
            AttivaDialogo();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        // Se il player si allontana mentre il popup è aperto, lo chiudiamo
        if (other.CompareTag("Player") && inDialogo && !isFollowing)
        {
            if (popupDialogo != null) popupDialogo.SetActive(false);
            inDialogo = false;
        }
    }

    // --- FUNZIONI UI ---

    public void AttivaDialogo()
    {
        isWaiting = false;
        inDialogo = true;
        if (popupDialogo != null) popupDialogo.SetActive(true);
    }

    public void FineDialogo() // Tasto SI
    {
        if (popupDialogo != null) popupDialogo.SetActive(false);
        inDialogo = false;
        isFollowing = true;
    }

    public void CliccatoNo() // Tasto NO
    {
        // 1. Chiudi il popup
        if (popupDialogo != null) popupDialogo.SetActive(false);

        // 2. Resetta lo stato di dialogo
        inDialogo = false;

        // 3. IMPOSTA IL TIMER: Per 2 secondi il cane ignorerà il player
        // Questo permette al cane di allontanarsi senza riaprire il popup
        timerIgnoraPlayer = 2.0f;
    }

    public void RestaQui()
    {
        if (isFollowing)
        {
            StartCoroutine(ShowAbandonMessage());
        }
        isFollowing = false;
        isWaiting = true;
    }

    IEnumerator ShowAbandonMessage()
    {
        if (popupAbbandono != null) popupAbbandono.SetActive(true);
        yield return new WaitForSeconds(4f);
        if (popupAbbandono != null) popupAbbandono.SetActive(false);
    }
}