using UnityEngine;

[RequireComponent(typeof(Potion))]
public class HealthPotion : MonoBehaviour
{
    [SerializeField] private float HealthAmount;
     
    private Potion Potion;
    private Player Player;
    private PotionDrinkCollider PotionDrinkCollider;

    private void Awake()
    {
        Player = FindObjectOfType<Player>();
        Potion = GetComponent<Potion>();
    }

    private void OnEnable()
    {
        //Subscribe "AddHealth" to potion effect event.
        Potion.effectEvent.AddListener(AddHealth);
    }

    //Function to add health to the player when this potion is pouring above the head.
    private void AddHealth()
    {
        if (Player != null && Player.Health < Player.MaxHealth && PotionDrinkCollider.IsColliding)
        {
            Player.Health = Mathf.Min(Player.MaxHealth, Player.Health + HealthAmount);
        }
    }
}
