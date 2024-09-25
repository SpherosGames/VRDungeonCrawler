using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grabbable : MonoBehaviour
{
    public HandPhysics hand;
    public Rigidbody rb;

    private void Awake()
    {
        if (rb == null) rb = GetComponent<Rigidbody>();
    }
}
