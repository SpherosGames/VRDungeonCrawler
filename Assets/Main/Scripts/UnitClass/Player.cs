using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Player : Unit
{
    [SerializeField] private string loseSceneName;
    [SerializeField] private Slider healthSlider;
    [SerializeField] private float strengthPotionTime = 30;

    [SerializeField] private List<float> strengthBuffs = new();

    public bool hasTempStrengthPotion;
    public float strengthPotionTimer;
    public float strength;

    private Sword sword;

    private void Awake()
    {
        sword = FindObjectOfType<Sword>();
    }

    private void OnEnable()
    {
        if (healthSlider) healthSlider.maxValue = MaxHealth;

        strength = 1;
    }

    public override void Update()
    {
        base.Update();

        if (healthSlider) healthSlider.value = Health;

        if (hasTempStrengthPotion)
        {
            strengthPotionTimer -= Time.deltaTime;

            if (strengthPotionTimer <= 0)
            {
                strengthPotionTimer = strengthPotionTime;
                hasTempStrengthPotion = false;
                CalculateStrength(0);
            }
        }
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

        CalculateStrength(amount);
    }

    private void CalculateStrength(float amount)
    {
        strength = 1;
        if (hasTempStrengthPotion) strength *= amount;
        for (int i = 0; i < strengthBuffs.Count; i++)
        {
            strength *= strengthBuffs[i];
        }

        if (sword) sword.SetDamageMultiplier(strength);
        else print("Didn't find sword, coulnd't set strength...");
    }
}
