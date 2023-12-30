using UnityEngine;

public class GameMenu : MonoBehaviour
{
    [SerializeField] private GameObject menuPanel;

    public void Menu()
    {
        menuPanel.SetActive(!menuPanel.activeSelf);
        
        if (menuPanel.activeSelf)
        {
            PauseManager.PauseGame();
        }
        else
        {
            CloseMenu();
        }
    }
    
    public void CloseMenu()
    {
        menuPanel.SetActive(false);
        PauseManager.UnPauseGame();
    }
}