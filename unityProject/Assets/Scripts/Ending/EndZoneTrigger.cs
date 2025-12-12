using UnityEngine;

public class EndZoneTrigger : MonoBehaviour
{
    public TimerManager timerManager;
    public EndScreenManager endScreenManager;

  
    public NewPlayerMovement playerMovementScript;

    private bool hasTriggered = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hasTriggered) return;
        if (!other.CompareTag("Player")) return;

        hasTriggered = true;

        string finalTime = "";

        if (timerManager != null)
        {
            finalTime = timerManager.GetCurrentTimeString();
        }

        if (playerMovementScript != null)
        {
            playerMovementScript.enabled = false;
        }

        if (endScreenManager != null)
        {
            endScreenManager.ShowEndScreen(finalTime);
        }

        
    }
}