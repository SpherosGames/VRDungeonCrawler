using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Potion))]
public class HealthPotion : MonoBehaviour
{
    [SerializeField] private float healthAmount;
     
    private Potion potion;
    private Player player;
    private PotionDrinkCollider potionDrinkCollider;

    private void Awake()
    {
        player = FindObjectOfType<Player>();
        potion = GetComponent<Potion>();
    }

    private void OnEnable()
    {
        potion.effectEvent.AddListener(AddHealth);
    }

    // Function to add health to the player when pouring above head
    private void AddHealth()
    {
        if (player != null && player.Health < player.MaxHealth && potionDrinkCollider.isColliding)
        {
            print("Actually heal");
            player.Health = Mathf.Min(player.MaxHealth, player.Health + healthAmount);
        }
    }
}
