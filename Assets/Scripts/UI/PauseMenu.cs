using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject pauseButton;
    [SerializeField] private GameObject pauseMenu;
    private bool paused = false;
    private bool timeIsFrozen = false;
    
    public bool TimeIsFrozen { set => timeIsFrozen = value; }

    // public void PlayPause()
    // {
    //     if (paused != paused)
    //     {
    //         if (timeIsFrozen) Time.timeScale = 0;
    //
    //         pauseButton.SetActive(false);
    //         pauseMenu.SetActive(true);
    //     }
    //     else
    //     {
    //         if (!timeIsFrozen) Time.timeScale = 1;
    //
    //         pauseButton.SetActive(true);
    //         pauseMenu.SetActive(false);
    //     }
    // }

    public void ClosePause()
    {
        paused = false;
        timeIsFrozen = false;
        Time.timeScale = 1;
        
        pauseButton.SetActive(true);
        pauseMenu.SetActive(false);
    }

    public void PlayPause()
    {
        paused = true;
        timeIsFrozen = true;
        Time.timeScale = 0;
        
        pauseButton.SetActive(false);
        pauseMenu.SetActive(true);
    }
}