using UnityEngine;

public class PotionDrinkCollider : MonoBehaviour
{
    public bool IsColliding {get; set;}

    //When an object with the "Potion" tag enters the head collider, it sets the "IsColliding" bool to true.
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Potion"))
        {
            IsColliding = true;
        }
    }

    //When an object with the "Potion" tag leaves the head collider, it sets the "IsColliding" bool to false.
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Potion"))
        {
            IsColliding = false;
        }
    }
}
