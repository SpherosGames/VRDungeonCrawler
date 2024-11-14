using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackCollision : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.gameObject.name);

        if (other.gameObject.layer == 12)
        {
            other.gameObject.GetComponentInParent<Unit>().TakeDamage(10);
        }
        if (other.gameObject.name == "Shield")
        {
            gameObject.GetComponentInParent<EnemyAttack>().Blocked();
        }
    }
}
