using UnityEngine;

public class Unit : MonoBehaviour
{
    public string Name;
    public float Health = 100f;
    public float MaxHealth = 100f;
    public float Speed = 5;

    public virtual void Update()
    {
        if (Health <= 0)
        {
            Die();
        }
    }
    public virtual void Die()
    {
        Debug.Log($"{Name} has died");
        Destroy(gameObject);
    }

    public virtual void TakeDamage(float damage)
    {
        Health = Mathf.Max(0, Health - damage);
        //Debug.Log($"{name} took {damage} damage. Current health: {Health}");
    }
    public virtual void DoDamage(float damage)
    {

    }

}