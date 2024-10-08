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
    public bool keepAwake;
    public float velocityThreshold = 0.01f;
    public float sleepDelay = 2f;

    private bool isSleeping = false;
    private float sleepTimer;

    private void Awake()
    {
        if (rb == null) rb = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        if (setCOM) rb.centerOfMass = transform.TransformPoint(grabPoint.position);
    }

    private void FixedUpdate()
    {
        if (isSleeping || isGrabbed || keepAwake) return;

        //print("Update go brr");
        //print(rb.velocity.magnitude);

        if (rb.velocity.magnitude < velocityThreshold)
        {
            sleepTimer += Time.fixedDeltaTime;
            if (sleepTimer >= sleepDelay)
            {
                //print("Sleep");
                rb.Sleep();
                isSleeping = true;
            }
        }
        else
        {
            sleepTimer = 0f;
        }
    }

    public void WakeUp()
    {
        print("Wake up");
        isSleeping = true;
    }
}
