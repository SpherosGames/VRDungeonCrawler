using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoolPotion : MonoBehaviour
{
    [SerializeField] private bool IsLatched = true;
     private float FluidLevel;
    Wobble wobblescript;

    void Start()
    {

       
    }


    void Update()
    {
        while (true)
        {
            wobblescript = GetComponentInChildren<Wobble>();
            FluidLevel = wobblescript.rend.material.GetFloat("_Fill");
            Debug.Log(FluidLevel);
        }
       
    }
}
