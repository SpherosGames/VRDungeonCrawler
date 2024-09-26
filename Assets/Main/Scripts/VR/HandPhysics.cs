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
    [SerializeField] private LayerMask grabLayerMask;
    [SerializeField] private Transform palmPos;
    [SerializeField] private float grabCheckSize = 0.1f;

    private Collider[] colliders;

    private FixedJoint fixedJoint;

    private Grabbable currentGrabbable;

    private bool mayGrab;

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

        if (!mayGrab)
        {
            if (!isGrabButtonPressed)
            {
                print("May grab again");
                mayGrab = true;
            }
            return;
        }

        //Check if this hand is trying to grab something, but isn't already holding something
        if (isGrabButtonPressed && !currentGrabbable)
        {
            //Look for all colliders in an area around the palm of the hand
            Collider[] colliders = Physics.OverlapSphere(palmPos.position, grabCheckSize, grabLayerMask, QueryTriggerInteraction.Ignore);

            print("Colliders length: " + colliders.Length);
            //print("Colliders length: " + colliders[0].tryget);

            //Check if there are any valid colliders and check if the first one has a grabbable script on it
            if (colliders.Length > 0 && colliders[0].TryGetComponent(out Grabbable grabbable))
            {
                print("Grab");

                //Set grabble hand
                currentGrabbable = grabbable;
                currentGrabbable.hand = this;

                if (currentGrabbable.socket)
                {
                    currentGrabbable.socket.UnSocketObject();
                }

                Rigidbody rb = colliders[0].attachedRigidbody;

                //Setup fixedjoint
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

                StartCoroutine(SetColliders(false, 0));
            }
        }
        //Releasing
        else if (!isGrabButtonPressed && currentGrabbable)
        {
            StartCoroutine(SetColliders(true, colliderEnableDelay));
            ForceRelease();
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

    public void ForceRelease()
    {
        currentGrabbable.hand = null;
        currentGrabbable = null;

        mayGrab = false;

        if (fixedJoint) Destroy(fixedJoint);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(palmPos.position, grabCheckSize);
    }
}
