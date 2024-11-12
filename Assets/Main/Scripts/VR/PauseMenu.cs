using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private InputActionProperty menuButton;
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private Vector3 spawnOffset;

    private GameObject spawnPauseMenu;

    private bool isOpen;

    private void Update()
    {
        //When the menu button is pressed
        if (menuButton.action.ReadValue<float>() > 0.5f)
        {
            if (isOpen)
            {
                ClosePauseMenu();
            }
            else
            {
                OpenPauseMenu();
            }
        }
    }

    private void OpenPauseMenu()
    {
        isOpen = true;

        ClosePauseMenu();

        spawnPauseMenu = Instantiate(pauseMenu, transform.position + spawnOffset, Quaternion.identity);
    }

    private void ClosePauseMenu()
    {
        isOpen = false;
        if (spawnPauseMenu) Destroy(spawnPauseMenu);
    }
}
