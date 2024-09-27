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
                mayGrab = true;
            }
            return;
        }

        if (isGrabButtonPressed && currentGrabbable && !useJoint)
        {
            print("AAAAAAAAAAAAAAAAA");

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
                    print("Set");
                }

                //Rigidbody rb = colliders[0].attachedRigidbody;
                
                StartCoroutine(SetColliders(false, 0));

                if (!useJoint) return;

                //Setup fixedjoint
                fixedJoint = palmPos.gameObject.AddComponent<FixedJoint>();
                fixedJoint.autoConfigureConnectedAnchor = false;
                fixedJoint.enablePreprocessing = false;

                Vector3 position;

                //When there is a grabpoint, set connectedanchor there instead
                if (grabbable.grabPoint != null)
                {
                    position = grabbable.grabPoint.position;
                }
                else
                {
                    position = palmPosTarget.position;
                }

                if (grabbable.rb)
                {
                    if (grabbable.grabPoint)
                    {
                        print("Grab point go brr");
                        //fixedJoint.connectedBody = grabbable.grabPoint.GetComponent<Rigidbody>();
                        fixedJoint.connectedBody = grabbable.GetComponent<Rigidbody>();
                        fixedJoint.connectedAnchor = grabbable.rb.transform.InverseTransformPoint(position);
                        //fixedJoint.axis = transform.forward;

                        Quaternion rot = Quaternion.FromToRotation(grabbable.transform.forward, transform.forward);
                        palmPos.rotation = rot * palmPos.rotation;
                    }
                    else
                    {
                        //fixedJoint.axis = transform.right;
                        fixedJoint.connectedBody = grabbable.rb;
                        fixedJoint.connectedAnchor = grabbable.rb.transform.InverseTransformPoint(position);
                    }   
                }
                else
                {
                    fixedJoint.connectedAnchor = position;
                }

                //if (grabbable.grabPoint != null)
                //{
                //    print("rotate grabbable");
                //    grabbable.transform.forward = grabbable.grabPoint.forward;
                //}
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

        //Phyics position
        palmPos.GetComponent<Rigidbody>().velocity = (palmPosTarget.position - palmPos.position) / Time.fixedDeltaTime;
    }

    public void ForceRelease()
    {
        print("Delete");
        currentGrabbable.hand = null;
        currentGrabbable = null;

        mayGrab = false;

        if (fixedJoint) Destroy(fixedJoint);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(palmPosTarget.position, grabCheckSize);
    }
}
