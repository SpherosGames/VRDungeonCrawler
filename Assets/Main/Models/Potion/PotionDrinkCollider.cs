using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PotionDrinkCollider : MonoBehaviour
{
    public bool isColliding;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Potion"))
        {
            isColliding = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Potion"))
        {
            isColliding = false;
        }
    }
}
