using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Player : Unit
{
    [SerializeField] private string loseSceneName;
    [SerializeField] private Slider healthSlider;

    private void OnEnable()
    {
        if (healthSlider) healthSlider.maxValue = MaxHealth;
    }

    public override void Update()
    {
        base.Update();

        if (healthSlider) healthSlider.value = Health;
    }

    public override void Die()
    {
        SceneManager.LoadScene(loseSceneName);
    }
}
