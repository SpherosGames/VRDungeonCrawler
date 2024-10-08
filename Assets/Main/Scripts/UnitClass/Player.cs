using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : Unit
{
    [SerializeField] private string loseSceneName;

    public override void Die()
    {
        SceneManager.LoadScene(loseSceneName);
    }
}
