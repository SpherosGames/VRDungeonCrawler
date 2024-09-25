using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;

public class VRSocket : MonoBehaviour
{
    [SerializeField] private SocketableType allowedSocketType;
    [SerializeField] private Transform socketPoint;
    [SerializeField] private bool turnOffCollider = true;

    private GameObject socketedObject;
    private Collider[] socketedObjectColliders;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Socketable socket))
        {
            if (socket.GetSocketableType() == allowedSocketType)
            {
                SocketObject(other.gameObject);
            }
        }
    }

    //private void OnTriggerExit(Collider other)
    //{
    //    if (other.TryGetComponent(out Socketable socket))
    //    {
    //        if (socket.GetSocketableType() == allowedSocketType)
    //        {
    //            UnSocketObject(other);
    //        }
    //    }
    //}

    //private void UnSocketObject(Collider other)
    //{

    //}

    private void SocketObject(GameObject _socketedObject)
    {
        print("Socketed");
        socketedObject = _socketedObject.gameObject;
        Grabbable grabbable = socketedObject.GetComponent<Grabbable>();
        //Release the socketable from the hand
        grabbable.hand.ForceRelease();

        //Set gravity of socketable off
        if (grabbable.rb)
        {
            grabbable.rb.velocity = Vector3.zero;
            grabbable.rb.angularVelocity = Vector3.zero;
            grabbable.rb.useGravity = false;
        }

        if (socketPoint) socketedObject.transform.position = socketPoint.position;
        else socketedObject.transform.position = transform.position;

        socketedObject.transform.rotation = socketPoint.transform.rotation;

        if (turnOffCollider)
        {
            socketedObjectColliders = socketedObject.GetComponentsInChildren<Collider>();

            for (int i = 0; i < socketedObjectColliders.Length; i++)
            {
                socketedObjectColliders[i].enabled = false;
            }
        }
    }
}
