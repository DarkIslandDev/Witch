using UnityEngine;

public class UpgradeMenu : MonoBehaviour
{
    [SerializeField] private GameObject upgradePanel;

    public void OpenPanel()
    {
        upgradePanel.SetActive(true);                
        PauseManager.PauseGame();
    }

    public void ClosePanel()
    {
        upgradePanel.SetActive(false);
        PauseManager.UnPauseGame();
    }
}