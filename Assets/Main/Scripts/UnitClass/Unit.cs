using UnityEngine;

public class Unit : MonoBehaviour
{
    public string Name;
    public float Health = 100f;
    public float Speed = 5;

    private void Update()
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
        Debug.Log($"{Name} took {damage} damage. Current health: {Health}");
    }
}