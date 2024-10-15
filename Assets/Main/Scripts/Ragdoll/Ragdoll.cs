using UnityEngine;

public class Ragdoll : MonoBehaviour
{
    public Transform torso; // The torso or central part of the ragdoll
    public Transform targetOrientation; // Desired upright orientation (e.g., Vector3.up)
    public float proportionalGain = 10f; // P (proportional) gain
    public float derivativeGain = 5f; // D (derivative) gain
    private Rigidbody torsoRb;
    private Quaternion previousRotation;
    void Start()
    {
        torsoRb = torso.GetComponent<Rigidbody>();
        previousRotation = torso.rotation;
    }
    void FixedUpdate()
    {
        // Calculate the rotational difference (error) between the current and target orientation
        Quaternion currentRotation = torso.rotation;
        Quaternion targetRotation = targetOrientation.rotation;

        // Proportional: Rotate towards the target
        Quaternion rotationError = targetRotation * Quaternion.Inverse(currentRotation);

        // Convert quaternion rotation error to a vector axis-angle form
        Vector3 axis;
        float angle;
        rotationError.ToAngleAxis(out angle, out axis);

        // Apply torque based on the proportional component
        Vector3 proportionalTorque = proportionalGain * axis * Mathf.Deg2Rad * angle;

        // Derivative: Damp the rotational velocity (angular velocity)
        Quaternion deltaRotation = currentRotation * Quaternion.Inverse(previousRotation);
        Vector3 angularVelocity = deltaRotation.eulerAngles / Time.fixedDeltaTime;
        Vector3 derivativeTorque = -derivativeGain * angularVelocity;

        // Apply total torque (P + D)
        torsoRb.AddTorque(proportionalTorque + derivativeTorque);

        // Store the current rotation as previous for the next frame
        previousRotation = currentRotation;
    }
}
