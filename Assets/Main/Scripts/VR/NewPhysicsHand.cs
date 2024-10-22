using UnityEngine;

public class NewPhysicsHand : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private PhysicMaterial handMaterial;
    [SerializeField] private Transform palmPos;
    [SerializeField] private Vector3 positionOffset;
    [SerializeField] private Vector3 rotationOffset;
    [SerializeField] private float followSpeed;
    [SerializeField] private float rotationSpeed;

    private void OnEnable()
    {
        //Set rigidbody
        if (rb == null) rb = GetComponent<Rigidbody>();

        transform.position = target.transform.position;
    }

    private void FixedUpdate()
    {
        MovementAndRotation();
    }

    private void MovementAndRotation()
    {
        // Position
        var positionWithOffset = target.TransformPoint(positionOffset);
        var distance = Vector3.Distance(positionWithOffset, transform.position);
        rb.velocity = (positionWithOffset - transform.position).normalized * followSpeed * distance * Time.deltaTime;

        // Rotation
        var rotationWithOffset = target.rotation * Quaternion.Euler(rotationOffset);
        var q = rotationWithOffset * Quaternion.Inverse(rb.rotation);
        q.ToAngleAxis(out float angle, out Vector3 axis);
        if (Mathf.Abs(axis.magnitude) != Mathf.Infinity)
        {
            if (angle > 180.0f) { angle -= 360.0f; }
            rb.angularVelocity = angle * Mathf.Deg2Rad * rotationSpeed * Time.deltaTime * axis;
        }
    }
}
