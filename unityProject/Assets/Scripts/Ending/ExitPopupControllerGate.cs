using UnityEngine;
using UnityEngine.Tilemaps;

public class ExitPopupControllerGate : MonoBehaviour
{
    [Header("Oggetti di gioco")]
    public Tilemap exitWallTilemap;          
    public TimerManager timerManager;         
    public ScoreManager scoreManager;          
    public MonoBehaviour playerMovementScript; 
    public Collider2D exitTriggerCollider;     

    private void OnEnable()
    {
        
        if (playerMovementScript != null)
        {
            playerMovementScript.enabled = false;
        }
    }

    private void OnDisable()
    {
        
        if (playerMovementScript != null)
        {
            playerMovementScript.enabled = true;
        }
    }

    public void OnContinueClicked()
    {
       
        gameObject.SetActive(false);   
    }


    public void OnExitClicked()
    {
        
        if (timerManager != null)
        {
            timerManager.StopTimer();
        }

        
        if (scoreManager != null)
        {
            scoreManager.UpdateScoreText(0);
        }

       
        if (exitWallTilemap != null)
        {
            exitWallTilemap.ClearAllTiles();
        }

       
        if (exitTriggerCollider != null)
        {
            exitTriggerCollider.enabled = false;
        }

        
        gameObject.SetActive(false);
    }
}