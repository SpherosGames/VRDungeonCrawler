using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private InputActionProperty MenuButton;
    [SerializeField] private GameObject PauseMenuObject;
    [SerializeField] private Transform playerCamera;
    [SerializeField] private float PauseMenuDistance = 2;
    [SerializeField] private Player Player;
    [SerializeField] private VRPlayerMovement PlayerMovement;

    private GameObject SpawnPauseMenu;

    private bool IsOpen;

    private void OnEnable()
    {
        MenuButton.action.performed += (InputAction.CallbackContext callBackContext) => MenuButtonPressed();
    }

    //Toggle the pause menu if the button is pressed.
    private void MenuButtonPressed()
    {
        if (IsOpen)
        {
            ClosePauseMenu();
        }
        else
        {
            OpenPauseMenu();
        }
    }

    private void OpenPauseMenu()
    {
        ClosePauseMenu();

        IsOpen = true;

        //Disallow the player moving and turning.
        PlayerMovement.MayTurn = false;
        PlayerMovement.MayMove = false;

        //Spawn the pause menu in front of the player.
        SpawnPauseMenu = Instantiate(PauseMenuObject, playerCamera.position + playerCamera.forward * PauseMenuDistance, Quaternion.identity);
        PauseMenuUI spawnPauseMenuUI = SpawnPauseMenu.GetComponentInChildren<PauseMenuUI>();

        //Set the stats text based on the stats of the player.
        string statsText = "Stats: \n";
        statsText += "Strength: " + Player.Strength + "\n";
        if (Player.HasTempStrengthPotion) statsText += "Strength potion time remaining: " + Player.StrengthPotionTimer + "\n";
        statsText += "Max health: " + Player.MaxHealth + "\n";
        statsText += "Health: " + Player.Health + "\n";

        //Setup the actual spawned pause menu.
        spawnPauseMenuUI.Setup(this, statsText);
    }

    /// <summary>
    /// Closes the pause menu.
    /// </summary>
    public void ClosePauseMenu()
    {
        IsOpen = false;

        //Allow the player to move and turn again.
        PlayerMovement.MayTurn = true;
        PlayerMovement.MayMove = true;

        //Destroy the pause menu.
        if (SpawnPauseMenu) Destroy(SpawnPauseMenu);
    }
}
