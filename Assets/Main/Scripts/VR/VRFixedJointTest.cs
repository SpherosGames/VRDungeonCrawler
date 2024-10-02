using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRFixedJointTest : MonoBehaviour
{
    public GameObject objectToAttach;
    public Vector3 desiredRotation;

    private void Start()
    {
        // Get the FixedJoint component
        FixedJoint fixedJoint = GetComponent<FixedJoint>();

        if (fixedJoint != null && objectToAttach != null)
        {
            // Set the rotation of the object to attach
            objectToAttach.transform.rotation = Quaternion.Euler(desiredRotation);

            // Attach the object to the fixed joint
            fixedJoint.connectedBody = objectToAttach.GetComponent<Rigidbody>();
        }
    }
}
