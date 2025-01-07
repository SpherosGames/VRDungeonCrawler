using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(HingeJoint))]
public class HingeJointHelper : MonoBehaviour
{
    private HingeJoint Joint;

    private void Awake()
    {
        Joint = GetComponent<HingeJoint>();
    }

    private void OnEnable()
    {
        Joint.enableCollision = false;

        if (Joint.useLimits)
        {
            //Some preset values that are optimal for good VR Physics interaction with joints. This to mostly eliminate jitter.
            JointLimits Limits = Joint.limits;
            Limits.bounceMinVelocity = 0.2f;
            Limits.contactDistance = 1f;
            Limits.bounciness = 0f;
            Joint.limits = Limits;
        }
    }
}
