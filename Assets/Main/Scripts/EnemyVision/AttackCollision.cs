using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackCollision : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.gameObject.name);

        if (other.gameObject.name == "Player")
        {
            other.gameObject.GetComponent<Unit>().TakeDamage(10);
        }
        if (other.gameObject.name == "Shield")
        {
            gameObject.GetComponentInParent<EnemyAttack>().Blocked();
        }
    }
}
