using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Player : Unit
{
    [SerializeField] private string loseSceneName;
    [SerializeField] private Slider healthSlider;

    public override void Die()
    {
        SceneManager.LoadScene(loseSceneName);
    }
}
