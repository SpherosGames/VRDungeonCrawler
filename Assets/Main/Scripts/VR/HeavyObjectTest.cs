using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeavyObjectTest : MonoBehaviour
{
    [SerializeField] private Vector3 intertiaTensor;

    private void OnEnable()
    {
        GetComponent<Rigidbody>().inertiaTensor = intertiaTensor;
    }
}
