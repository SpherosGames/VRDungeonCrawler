using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenuUI : MonoBehaviour
{
    [SerializeField] private Button BackToGameButton;
    [SerializeField] private Button BackToMenuButton;
    [SerializeField] private TMP_Text StatsText;

    private PauseMenu PauseMenu;

    private void OnEnable()
    {
        //Setup buttons
        BackToGameButton.onClick.RemoveAllListeners();
        BackToGameButton.onClick.AddListener(BackToGame);

        BackToMenuButton.onClick.RemoveAllListeners();
        BackToMenuButton.onClick.AddListener(BackToMenu);
    }

    //Activates when the back to game button is pressed.
    private void BackToGame()
    {
        PauseMenu.ClosePauseMenu();
    }

    //Activates when the back to menu button is pressed.
    private void BackToMenu()
    {
        //Loads the first scene, the main menu.
        SceneManager.LoadScene(0);
    }
    
    //Setup the spawned pause menu.
    public void Setup(PauseMenu _pauseMenu, string _statsText)
    {
        PauseMenu = _pauseMenu;
        StatsText.SetText(_statsText);
    }
}
