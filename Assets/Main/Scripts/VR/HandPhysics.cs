using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class HandPhysics : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private PhysicMaterial handMaterial;
    [SerializeField] private float colliderEnableDelay = 1;
    [SerializeField] private InputActionProperty grabButton;
    [SerializeField] private float grabThreshold = 0.1f;
    [SerializeField] private Transform palmPos;
    [SerializeField] private float grabCheckSize = 0.1f;
    [SerializeField] private LayerMask grabLayer;

    private Collider[] colliders;

    private FixedJoint fixedJoint;

    private bool isGrabbing;

    private void Awake()
    {
        colliders = GetComponentsInChildren<Collider>();
        colliders.ToList().ForEach(x => x.material = handMaterial);
    }

    private void OnEnable()
    {
        //Set rigidbody
        if (rb == null) rb = GetComponent<Rigidbody>();
    }

    private IEnumerator SetColliders(bool value, float delay)
    {
        yield return new WaitForSeconds(delay);

        foreach (var item in colliders)
        {
            item.enabled = value;
        }
    }

    private void FixedUpdate()
    {
        MovementAndRotation();

        bool isGrabButtonPressed = grabButton.action.ReadValue<float>() > grabThreshold;

        if (isGrabButtonPressed && !isGrabbing)
        {
            Collider[] colliders = Physics.OverlapSphere(palmPos.position, grabCheckSize, grabLayer, QueryTriggerInteraction.Ignore);

            if (colliders.Length > 0)
            {
                Rigidbody rb = colliders[0].attachedRigidbody;

                fixedJoint = gameObject.AddComponent<FixedJoint>();
                fixedJoint.autoConfigureConnectedAnchor = false;

                if (rb)
                {
                    fixedJoint.connectedBody = rb;
                    fixedJoint.connectedAnchor = rb.transform.InverseTransformPoint(palmPos.position);
                }
                else
                {
                    fixedJoint.connectedAnchor = palmPos.position;
                }

                isGrabbing = true;
                StartCoroutine(SetColliders(false, 0));
            }
        }
        else if (!isGrabButtonPressed && isGrabbing)
        {
            StartCoroutine(SetColliders(true, colliderEnableDelay));
            isGrabbing = false;

            if (fixedJoint) Destroy(fixedJoint);
        }
    }

    private void MovementAndRotation()
    {
        //Phyics position
        rb.velocity = (target.position - transform.position) / Time.fixedDeltaTime;

        //Physics rotation
        Quaternion rotationDifference = target.rotation * Quaternion.Inverse(transform.rotation);
        rotationDifference.ToAngleAxis(out float angle, out Vector3 axis);

        Vector3 rotation = angle * axis;

        rb.angularVelocity = (rotation * Mathf.Deg2Rad) / Time.fixedDeltaTime;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(palmPos.position, grabCheckSize);
    }
}
