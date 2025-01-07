using UnityEngine;

public class StrengthPotion : MonoBehaviour
{
    [SerializeField] private float StrengthMultiplier;

    private Potion Potion;
    private Player Player;
    private PotionDrinkCollider PotionDrinkCollider;

    private void Awake()
    {
        Player = FindObjectOfType<Player>();
        Potion = GetComponent<Potion>();
        PotionDrinkCollider = FindObjectOfType<PotionDrinkCollider>();
    }

    private void OnEnable()
    {
        Potion.effectEvent.RemoveAllListeners();

        //Subscribe "AddStrength" to potion effect event.
        Potion.effectEvent.AddListener(AddStrength);
    }

    //Function to add health to the player when this potion is pouring above the head.
    private void AddStrength()
    {
        if (Player != null && Player.Health < Player.MaxHealth && PotionDrinkCollider.IsColliding)
        {
            Player.AddStrength(StrengthMultiplier, false);
        }
    }
}
