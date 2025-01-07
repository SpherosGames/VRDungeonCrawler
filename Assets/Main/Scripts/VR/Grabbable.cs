using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grabbable : MonoBehaviour
{
    public NewPhysicsHand Hand;
    public Rigidbody Rb;
    public VRSocket Socket;
    public Transform GrabPoint;
    public bool TwoHanded = false;
    public bool IsGrabbed;
    public bool SetCOM;
    public bool KeepAwake;
    public float VelocityThreshold = 0.01f;
    public float SleepDelay = 2f;
    public bool OptimizePhysics = false;

    private float CheckTimer;
    private float CheckTime = 0.5f;
    private float AcceptDistance = 1;

    private Transform Player;

    private bool IsSleeping = false;
    private float SleepTimer;

    private void Awake()
    {
        if (Rb == null) Rb = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        if (SetCOM) Rb.centerOfMass = transform.TransformPoint(GrabPoint.position);

        if (OptimizePhysics)
        {
            Rb.collisionDetectionMode = CollisionDetectionMode.Discrete;
            CheckTimer = CheckTime;
            Player = FindObjectOfType<Player>().transform;
        }
    }

    private void FixedUpdate()
    {
        if (CheckTimer > 0)
        {
            CheckTimer -= Time.deltaTime;

            //Check every "CheckTime" seconds for the distance between the object and the player and make it isKinematic if it's close enough.
            if (CheckTimer <= 0)
            {
                float dist = Vector3.Distance(Player.transform.position, transform.position);

                if (dist < AcceptDistance)
                {
                    Rb.isKinematic = false;
                }
                else
                {
                    Rb.isKinematic = true;
                }

                CheckTimer = CheckTime;
            }
        }

        if (IsSleeping || IsGrabbed || KeepAwake) return;

        //If the object is moving very slowly for "SleepDelay" seconds the rigidbody turns itself off by calling the Sleep function.
        if (Rb.velocity.magnitude < VelocityThreshold)
        {
            SleepTimer += Time.fixedDeltaTime;
            if (SleepTimer >= SleepDelay)
            {
                Rb.Sleep();
                IsSleeping = true;
            }
        }
        else
        {
            SleepTimer = 0f;
        }
    }

    /// <summary>
    /// Wakes up the Grabbables Rigidbody.
    /// </summary>
    public void WakeUp()
    {
        Rb.WakeUp();
        IsSleeping = true;
    }
}
