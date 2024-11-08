using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBlocking : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Sword"))
        {
            Debug.Log("The enemy blocked you!!!!!");
        }
    }
}
