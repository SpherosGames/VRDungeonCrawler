using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grabbable : MonoBehaviour
{
    public HandPhysics hand;
    public Rigidbody rb;
    public VRSocket socket;
    public Transform grabPoint;
    public bool twoHanded = false;
    public bool isGrabbed;
    public bool setCOM;

    private void Awake()
    {
        if (rb == null) rb = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        if (setCOM) rb.centerOfMass = transform.TransformPoint(grabPoint.position);
    }
}
