using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Player : Unit
{
    [SerializeField] private string loseSceneName;
    [SerializeField] private Slider healthSlider;

    [SerializeField] private List<float> strengthBuffs = new();

    private bool hasTempStrengthPotion;
    private float tempStrengAmount = 1.1f;
    private float strength;

    private void OnEnable()
    {
        if (healthSlider) healthSlider.maxValue = MaxHealth;
    }

    public override void Update()
    {
        base.Update();

        if (healthSlider) healthSlider.value = Health;

        //if (hasTempStrengthPotion) 
    }

    public override void Die()
    {
        SceneManager.LoadScene(loseSceneName);
    }

    public void AddStrength(float amount, bool isPermanent)
    {
        if (isPermanent)
        {
            strengthBuffs.Add(amount);
        }
        else
        {
            hasTempStrengthPotion = true;
        }

        strength = 1;
        if (hasTempStrengthPotion) strength *= tempStrengAmount;
        //for (int i = 0; i < length; i++)
        //{

        //}
    }
}
