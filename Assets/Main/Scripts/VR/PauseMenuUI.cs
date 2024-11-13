using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenuUI : MonoBehaviour
{
    [SerializeField] private Button backToGameButton;
    [SerializeField] private Button backToMenuButton;
    [SerializeField] private TMP_Text statsText;

    private PauseMenu pauseMenu;

    private void OnEnable()
    {
        backToGameButton.onClick.RemoveAllListeners();
        backToGameButton.onClick.AddListener(BackToGame);

        backToMenuButton.onClick.RemoveAllListeners();
        backToMenuButton.onClick.AddListener(BackToMenu);
    }

    private void BackToGame()
    {
        pauseMenu.ClosePauseMenu(true);
    }

    private void BackToMenu()
    {
        SceneManager.LoadScene(0);
    }
    
    //TODO: Stats
    public void Setup(PauseMenu _pauseMenu, string _statsText)
    {
        pauseMenu = _pauseMenu;
        statsText.SetText(_statsText);
    }
}
