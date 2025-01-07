using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Player : Unit
{
    [SerializeField] private string LoseSceneName;
    [SerializeField] private Slider HealthSlider;
    [SerializeField] private float StrengthPotionTime = 30;

    [SerializeField] private List<float> StrengthBuffs = new();

    public bool HasTempStrengthPotion;
    public float StrengthPotionTimer;
    public float Strength;

    private Sword Sword;

    private void Awake()
    {
        Sword = FindObjectOfType<Sword>();
    }

    private void OnEnable()
    {
        if (HealthSlider) HealthSlider.maxValue = MaxHealth;

        Strength = 1;
    }

    public override void Update()
    {
        base.Update();

        if (HealthSlider) HealthSlider.value = Health;

        //Remove the effect of the temp strength potion after "StrengthPotionTimer" seconds.
        if (HasTempStrengthPotion)
        {
            StrengthPotionTimer -= Time.deltaTime;

            if (StrengthPotionTimer <= 0)
            {
                StrengthPotionTimer = StrengthPotionTime;
                HasTempStrengthPotion = false;
                CalculateStrength(0);
            }
        }
    }

    public override void Die()
    {
        SceneManager.LoadScene(LoseSceneName);
    }

    /// <summary>
    /// Adds strength to the players sword.
    /// </summary>
    /// <param name="Amount">The multiplier for the strength. For example: 1.2 increases the strength of the player by 20%.</param>
    /// <param name="IsPermanent">Wether or not the strength buff is permanent or not. If not it will be removed in "StrengthPotionTimer" seconds.</param>
    public void AddStrength(float Amount, bool IsPermanent)
    {
        if (IsPermanent)
        {
            StrengthBuffs.Add(Amount);
        }
        else
        {
            HasTempStrengthPotion = true;
        }

        CalculateStrength(Amount);
    }

    private void CalculateStrength(float amount)
    {
        Strength = 1;
        if (HasTempStrengthPotion) Strength *= amount;
        for (int i = 0; i < StrengthBuffs.Count; i++)
        {
            Strength *= StrengthBuffs[i];
        }

        //if (sword) sword.SetDamageMultiplier(strength);
        //else print("Didn't find sword, coulnd't set strength...");
    }
}
