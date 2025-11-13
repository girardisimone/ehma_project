using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ExitPopup : MonoBehaviour
{
    [Header("Riferimenti UI")]
    [SerializeField] private GameObject root;        // il GameObject "ExitPopup" (pannello intero) che parte disattivo
    [SerializeField] private Button exitButton;      // Button_Exit
    [SerializeField] private Button continueButton;  // Button_Continue

    public UnityEvent OnChooseExit;       // verrà ascoltato dal gate
    public UnityEvent OnChooseContinue;   // idem

    private bool isOpen;
    private float prevTimeScale = 1f;

    private void Awake()
    {
        // Collega i bottoni una sola volta
        if (exitButton != null)
            exitButton.onClick.AddListener(() =>
            {
                OnChooseExit?.Invoke();
                Hide();
            });

        if (continueButton != null)
            continueButton.onClick.AddListener(() =>
            {
                OnChooseContinue?.Invoke();
                Hide();
            });

        HideInstant();
    }

    public void Show()
    {
        if (isOpen) return;
        isOpen = true;
        if (root != null) root.SetActive(true);

        // pausa opzionale, ferma il movimento mentre il popup è aperto
        prevTimeScale = Time.timeScale;
        Time.timeScale = 0f;
    }

    public void Hide()
    {
        if (!isOpen) return;
        isOpen = false;
        if (root != null) root.SetActive(false);
        Time.timeScale = prevTimeScale;
    }

    public void HideInstant()
    {
        isOpen = false;
        if (root != null) root.SetActive(false);
    }
}
