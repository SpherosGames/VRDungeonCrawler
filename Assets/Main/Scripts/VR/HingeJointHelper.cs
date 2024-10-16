using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(HingeJoint))]
public class HingeJointHelper : MonoBehaviour
{
    private HingeJoint joint;

    private void Awake()
    {
        joint = GetComponent<HingeJoint>();
    }

    private void OnEnable()
    {
        joint.enableCollision = false;

        if (joint.useLimits)
        {
            JointLimits limits = joint.limits;
            limits.bounceMinVelocity = 0.2f;
            limits.contactDistance = 1f;
            limits.bounciness = 0f;
            joint.limits = limits;
        }
    }
}
