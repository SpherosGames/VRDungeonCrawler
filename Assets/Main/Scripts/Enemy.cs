using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : Unit
{
    [SerializeField] private Slider healthSlider;

    private void OnEnable()
    {
        healthSlider.maxValue = MaxHealth;
    }

    public override void Update()
    {
        base.Update();

        healthSlider.value = Health;
    }

}
