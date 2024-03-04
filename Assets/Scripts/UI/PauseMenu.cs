using System;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private LevelManager levelManager;
    
    [SerializeField] private GameObject pauseButton;
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject settingsMenu;

    [SerializeField] private LocalizedText pauseText;
    [SerializeField] private LocalizedText settingsText;
    [SerializeField] private LocalizedText continueText;
    [SerializeField] private LocalizedText restartText;
    [SerializeField] private LocalizedText exitText;

    public bool paused = false;
    private bool timeIsFrozen = false;

    private void Start()
    {
        OnLanguageChange();
        LocalizationManager.OnLanguageChange += OnLanguageChange;
    }

    private void OnDestroy() => LocalizationManager.OnLanguageChange -= OnLanguageChange;

    public bool TimeIsFrozen
    {
        set => timeIsFrozen = value;
    }

    public void Open()
    {
        pauseButton.SetActive(false);
        pauseMenu.SetActive(true);
    }

    public void Close()
    {
        pauseButton.SetActive(true);
        pauseMenu.SetActive(false);
    }

    public void ClosePause()
    {
        levelManager.gameState = GameState.Game;
        levelManager.SwitchGameState();

        pauseButton.SetActive(true);
        pauseMenu.SetActive(false);
    }

    public void PlayPause()
    {
        levelManager.gameState = GameState.Pause;
        levelManager.SwitchGameState();
        
        pauseButton.SetActive(false);
        pauseMenu.SetActive(true);
    }

    public void Pause(bool enable, int time)
    {
        paused = enable;
        timeIsFrozen = enable;
        Time.timeScale = time;
    }

    public void TakePause(bool enable)
    {
        if (enable)
        {
            switch (levelManager.gameState)
            {
                case GameState.Pause:
                    ClosePause();
                    break;
                case GameState.Game:
                    PlayPause();
                    break;
                case GameState.LevelUp:
                    break;
                case GameState.GameOver:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    public void OpenSettings()
    {
        pauseMenu.SetActive(false);
        settingsMenu.SetActive(true);
    }

    public void CloseSettings()
    {
        settingsMenu.SetActive(false);
        pauseMenu.SetActive(true);
    }

    private void OnLanguageChange()
    {
        pauseText.Localize("pause_key");
        settingsText.Localize("settings_key");
        continueText.Localize("continue_key");
        restartText.Localize("retry_key");
        exitText.Localize("exit_key");
    }
}