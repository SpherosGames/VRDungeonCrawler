using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixedJointTest : MonoBehaviour
{
    public Vector3 axis;

    private void Update()
    {
        GetComponent<FixedJoint>().axis = axis;
    }
}
