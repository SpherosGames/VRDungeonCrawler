using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantKill : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.parent.gameObject.GetComponent<Unit>())
        {
            other.transform.parent.gameObject.GetComponent<Unit>().TakeDamage(9999999999999999999);
        }
    }
}
