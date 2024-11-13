using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private InputActionProperty menuButton;
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private Transform playerCamera;
    [SerializeField] private float pauseMenuDistance = 2;
    [SerializeField] private Player player;
    [SerializeField] private VRPlayerMovement playerMovement;

    private GameObject spawnPauseMenu;

    private bool isOpen;

    private void OnEnable()
    {
        menuButton.action.performed += (InputAction.CallbackContext callBackContext) => MenuButtonPressed(callBackContext);
    }

    private void MenuButtonPressed(InputAction.CallbackContext callBackContext)
    {
        if (isOpen)
        {
            ClosePauseMenu(true);
        }
        else
        {
            OpenPauseMenu();
        }
    }

    private void OpenPauseMenu()
    {
        ClosePauseMenu(false);

        isOpen = true;

        playerMovement.mayTurn = false;
        playerMovement.mayMove = false;

        spawnPauseMenu = Instantiate(pauseMenu, playerCamera.position + playerCamera.forward * pauseMenuDistance, Quaternion.identity);
        PauseMenuUI spawnPauseMenuUI = spawnPauseMenu.GetComponentInChildren<PauseMenuUI>();

        string statsText = "Stats: \n";
        statsText += "Strength: " + player.strength + "\n";
        if (player.hasTempStrengthPotion) statsText += "Strength potion time remaining: " + player.strengthPotionTimer + "\n";
        statsText += "Max health: " + player.MaxHealth + "\n";
        statsText += "Health: " + player.Health + "\n";

        spawnPauseMenuUI.Setup(this, statsText);
    }

    public void ClosePauseMenu(bool open)
    {
        isOpen = false;

        playerMovement.mayTurn = true;
        playerMovement.mayMove = true;

        if (spawnPauseMenu) Destroy(spawnPauseMenu);
    }
}
