using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net.WebSockets;
using UnityEngine;

public class VRSocket : MonoBehaviour
{
    [SerializeField] private SocketableType allowedSocketType;
    [SerializeField] private Transform socketPoint;
    [SerializeField] private float reSocketDelay = 1f;

    private bool maySocket;
    private float reSocketTimer;

    private GameObject socketedObject;
    private Grabbable socketedGrabbable;

    private void OnTriggerEnter(Collider other)
    {
        if (!maySocket) return;

        if (other.TryGetComponent(out Socketable socket))
        {
            if (socket.GetSocketableType() == allowedSocketType)
            {
                SocketObject(other.gameObject);
            }
        }
    }

    private void Update()
    {
        if (!maySocket)
        {
            reSocketTimer -= Time.deltaTime;

            if (reSocketTimer <= 0)
            {
                maySocket = true;
                reSocketTimer = reSocketDelay;
            }
        }
    }

    public void UnSocketObject()
    {
        if (socketedGrabbable.rb)
        {
            socketedGrabbable.rb.isKinematic = false;
        }

        socketedGrabbable.socket = null;
        socketedGrabbable = null;

        socketedObject = null;
    }

    private void SocketObject(GameObject _socketedObject)
    {
        //sockettimer to make sure the object doenst get immmediatly resocketed;
        reSocketTimer = reSocketDelay;
        maySocket = false;

        socketedObject = _socketedObject.gameObject;
        socketedGrabbable = socketedObject.GetComponent<Grabbable>();
        //Release the socketable from the hand
        socketedGrabbable.hand.ForceRelease();

        //Set gravity of socketable off
        if (socketedGrabbable.rb)
        {
            socketedGrabbable.rb.velocity = Vector3.zero;
            socketedGrabbable.rb.angularVelocity = Vector3.zero;
            socketedGrabbable.rb.isKinematic = true;
        }
        else
        {
            Debug.Log("WARNING No rigidbody found, can't socket object", socketedObject);
        }

        socketedGrabbable.socket = this;

        if (socketPoint) socketedObject.transform.position = socketPoint.position;
        else socketedObject.transform.position = transform.position;

        socketedObject.transform.rotation = socketPoint.transform.rotation;
    }
}
