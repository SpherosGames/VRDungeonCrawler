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
    [SerializeField] private bool useJoint;
    [SerializeField] private Transform palmPosTarget;

    private Collider[] colliders;

    private FixedJoint fixedJoint;

    private Grabbable currentGrabbable;

    private Grabbable socketedObject;

    private bool mayGrab = true;

    private void Awake()
    {
        colliders = GetComponentsInChildren<Collider>();
        colliders.ToList().ForEach(x => x.material = handMaterial);
    }

    private void OnEnable()
    {
        //Set rigidbody
        if (rb == null) rb = GetComponent<Rigidbody>();

        transform.position = target.transform.position;
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

        //MovePalm();

        Grabbing();

        //LimitPalmSpeed();
    }

    private void LimitPalmSpeed()
    {
        Rigidbody rb = palmPos.GetComponent<Rigidbody>();

        if (rb.velocity.magnitude > 10)
        {
            rb.velocity = rb.velocity.normalized * 10;
        }
        if (rb.angularVelocity.magnitude > 10)
        {
            rb.angularVelocity = rb.angularVelocity.normalized * 10;
        }
    }

    private void Grabbing()
    {
        bool isGrabButtonPressed = grabButton.action.ReadValue<float>() > grabThreshold;

        if (!mayGrab)
        {
            if (!isGrabButtonPressed)
            {
                mayGrab = true;
            }
            return;
        }

        //TODO: Remove this if im sure fixed joint are the way to go
        if (isGrabButtonPressed && currentGrabbable && !useJoint)
        {
            Transform target;

            //Set movement and rotation target to grabpoint or object itself
            if (currentGrabbable.grabPoint)
            {
                target = currentGrabbable.grabPoint;
            }
            else
            {
                target = currentGrabbable.transform;
            }

            //Move object towards palm of hand
            currentGrabbable.rb.velocity = (palmPos.position - target.position).normalized * 10
                * Vector3.Distance(palmPos.position, target.position);

            //Rotate the object towards the palm of the hand
            Quaternion rotationDifference = palmPos.rotation * Quaternion.Inverse(target.rotation);

            rotationDifference.ToAngleAxis(out float angle, out Vector3 axis);
            Vector3 angularVelocity = 10 * angle * Mathf.Deg2Rad * axis;

            currentGrabbable.rb.angularVelocity = angularVelocity;
        }

        //Check if this hand is trying to grab something, but isn't already holding something
        if (isGrabButtonPressed && !currentGrabbable)
        {
            if (socketedObject)
            {
                if (Vector3.Distance(transform.TransformPoint(palmPosTarget.position), socketedObject.transform.position) >= socketedObject.socket.ReleaseDistance)
                {
                    currentGrabbable = socketedObject;
                    currentGrabbable.hand = this;
                    currentGrabbable.socket.UnSocketObject();
                    socketedObject = null;
                    return;
                }
            }

            //Look for all colliders in an area around the palm of the hand
            Collider[] colliders = Physics.OverlapSphere(palmPosTarget.position, grabCheckSize, grabLayerMask, QueryTriggerInteraction.Ignore);

            //Check if there are any valid colliders and check if the first one has a grabbable script on it
            if (colliders.Length > 0 && colliders[0].TryGetComponent(out Grabbable grabbable))
            {
                if (grabbable.isGrabbed && !grabbable.twoHanded) return;

                grabbable.isGrabbed = true;

                //Socket
                if (grabbable.socket)
                {
                    socketedObject = grabbable;
                }
                else
                {
                    //Set grabble hand
                    currentGrabbable = grabbable;
                    currentGrabbable.hand = this;
                }

                StartCoroutine(SetColliders(false, 0));

                if (!useJoint) return;

                //Setup fixedjoint
                fixedJoint = gameObject.AddComponent<FixedJoint>();
                fixedJoint.autoConfigureConnectedAnchor = false;

                Vector3 position;

                //When there is a grabpoint, set connectedanchor there instead
                if (grabbable.grabPoint != null)
                {
                    //position = grabbable.grabPoint.position;
                    position = palmPosTarget.position;
                }
                else
                {
                    position = palmPosTarget.position;
                }

                if (grabbable.rb)
                {
                    grabbable.rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

                    if (grabbable.grabPoint)
                    {
                        // Store the original position of the grabbable object
                        Vector3 originalPosition = grabbable.transform.position;

                        // Calculate and apply rotation
                        Quaternion rotationDifference = transform.rotation * Quaternion.Inverse(grabbable.grabPoint.rotation);
                        grabbable.transform.rotation = rotationDifference * grabbable.transform.rotation;

                        // Calculate the offset from the grab point to the object's center
                        Vector3 grabOffset = grabbable.transform.position - grabbable.grabPoint.position;

                        // Calculate the new position
                        Vector3 newPosition = position + transform.rotation * Quaternion.Inverse(grabbable.grabPoint.rotation) * grabOffset;

                        // Set the object's position
                        grabbable.transform.position = newPosition;

                        // Set up the joint
                        fixedJoint.connectedBody = grabbable.rb;
                        fixedJoint.connectedAnchor = grabbable.rb.transform.InverseTransformPoint(position);

                        //Uncomment this when it's not done in fixedupdate (MovementAndRotation)
                        //Quaternion rot = Quaternion.FromToRotation(grabbable.grabPoint.transform.forward, transform.forward);
                        //palmRot = rot * palmPos.rotation;
                    }
                    else
                    {
                        fixedJoint.connectedBody = grabbable.rb;
                        fixedJoint.connectedAnchor = grabbable.rb.transform.InverseTransformPoint(position);
                    }
                }
                else
                {
                    fixedJoint.connectedAnchor = position;
                }
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

    private void MovePalm()
    {
        //move palmpos to it's target
        palmPos.position = palmPosTarget.position;

        //rotate palmpos to it's target
        if (currentGrabbable && currentGrabbable.grabPoint)
        {
            //TODO: optimize this by calculating an offset and using localrotation instead
            Quaternion rot = Quaternion.FromToRotation(currentGrabbable.grabPoint.transform.forward, transform.forward);
            Quaternion worldRot = rot * palmPos.rotation;

            palmPos.rotation = worldRot;
        }
        else
        {
            palmPos.localRotation = Quaternion.identity;
        }
    }

    public void ForceRelease()
    {
        currentGrabbable.rb.collisionDetectionMode = CollisionDetectionMode.Discrete;
        currentGrabbable.isGrabbed = false;

        currentGrabbable.hand = null;
        currentGrabbable = null;

        mayGrab = false;

        if (fixedJoint) Destroy(fixedJoint);
    }

    private void OnDrawGizmosSelected()
    {
        if (palmPosTarget) Gizmos.DrawWireSphere(palmPosTarget.position, grabCheckSize);
    }
}
