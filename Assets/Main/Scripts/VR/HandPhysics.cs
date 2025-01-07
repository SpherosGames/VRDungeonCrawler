using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class HandPhysics : MonoBehaviour
{
    [SerializeField] private Transform Target;
    [SerializeField] private Rigidbody Rb;
    [SerializeField] private PhysicMaterial HandMaterial;
    [SerializeField] private float ColliderEnableDelay = 1;
    [SerializeField] private InputActionProperty GrabButton;
    [SerializeField] private float GrabThreshold = 0.1f;
    [SerializeField] private LayerMask GrabLayerMask;
    [SerializeField] private Transform PalmPos;
    [SerializeField] private float GrabCheckSize = 0.1f;
    [SerializeField] private bool UseJoint;
    [SerializeField] private Transform PalmPosTarget;

    private Collider[] HandColliders;

    private FixedJoint FixedJoint;

    private Grabbable CurrentGrabbable;

    private Grabbable SocketedObject;

    private bool MayGrab = true;

    private void Awake()
    {
        //Get all hand colliders and set the hand physics material.
        HandColliders = GetComponentsInChildren<Collider>();
        HandColliders.ToList().ForEach(X => X.material = HandMaterial);
    }

    private void OnEnable()
    {
        //Find and assign Rigidbody.
        if (Rb == null) Rb = GetComponent<Rigidbody>();

        //Teleport the physic hand to it's target position.
        transform.position = Target.transform.position;
    }

    /// <summary>
    /// Set the all the hand colliders based on "value".
    /// </summary>
    /// <param name="Value">Whether or not the hand colliders are on.</param>
    /// <param name="Delay">How long it waits to change the state of the hand colliders.</param>
    /// <returns></returns>
    private IEnumerator SetColliders(bool Value, float Delay)
    {
        yield return new WaitForSeconds(Delay);

        foreach (var item in HandColliders)
        {
            item.enabled = Value;
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
        Rigidbody Rb = PalmPos.GetComponent<Rigidbody>();

        if (Rb.velocity.magnitude > 10)
        {
            Rb.velocity = Rb.velocity.normalized * 10;
        }
        if (Rb.angularVelocity.magnitude > 10)
        {
            Rb.angularVelocity = Rb.angularVelocity.normalized * 10;
        }
    }

    private void Grabbing()
    {
        //Whether or not the grab button is pressed.
        bool IsGrabButtonPressed = GrabButton.action.ReadValue<float>() > GrabThreshold;

        //You may only grab an object again if you first stop pressing the grabbutton.
        if (!MayGrab)
        {
            if (!IsGrabButtonPressed)
            {
                MayGrab = true;
            }
            return;
        }

        //TODO: Remove this if im sure fixed joint are the way to go.
        if (IsGrabButtonPressed && CurrentGrabbable && !UseJoint)
        {
            Transform Target;

            //Set movement and rotation target to grabpoint or object itself.
            if (CurrentGrabbable.GrabPoint)
            {
                Target = CurrentGrabbable.GrabPoint;
            }
            else
            {
                Target = CurrentGrabbable.transform;
            }

            //Move object towards palm of hand.
            CurrentGrabbable.Rb.velocity = (PalmPos.position - Target.position).normalized * 10
                * Vector3.Distance(PalmPos.position, Target.position);

            //Rotate the object towards the palm of the hand.
            Quaternion RotationDifference = PalmPos.rotation * Quaternion.Inverse(Target.rotation);

            RotationDifference.ToAngleAxis(out float Angle, out Vector3 Axis);
            Vector3 AngularVelocity = 10 * Angle * Mathf.Deg2Rad * Axis;

            CurrentGrabbable.Rb.angularVelocity = AngularVelocity;
        }

        //Check if this hand is trying to grab something, but isn't already holding something.
        if (IsGrabButtonPressed && !CurrentGrabbable)
        {
            if (SocketedObject)
            {
                if (Vector3.Distance(transform.TransformPoint(PalmPosTarget.position), SocketedObject.transform.position) >= SocketedObject.Socket.GetReleaseDistance)
                {
                    CurrentGrabbable = SocketedObject;
                    //currentGrabbable.hand = this;
                    CurrentGrabbable.Socket.UnSocketObject(SocketedObject.gameObject);
                    SocketedObject = null;
                    return;
                }
            }

            //Look for all colliders in an area around the palm of the hand.
            Collider[] Colliders = Physics.OverlapSphere(PalmPosTarget.position, GrabCheckSize, GrabLayerMask, QueryTriggerInteraction.Ignore);

            //Check if there are any valid colliders and check if the first one has a grabbable script on it.
            if (Colliders.Length > 0 && Colliders[0].TryGetComponent(out Grabbable Grabbable))
            {
                if (Grabbable.IsGrabbed && !Grabbable.TwoHanded) return;

                Grabbable.IsGrabbed = true;

                //Socket.
                if (Grabbable.Socket)
                {
                    SocketedObject = Grabbable;
                }
                else
                {
                    //Set grabble hand.
                    CurrentGrabbable = Grabbable;
                }

                StartCoroutine(SetColliders(false, 0));

                if (!UseJoint) return;

                //Setup fixedjoint.
                FixedJoint = gameObject.AddComponent<FixedJoint>();
                FixedJoint.autoConfigureConnectedAnchor = false;

                Vector3 Position;

                //When there is a grabpoint, set connectedanchor there instead.
                if (Grabbable.GrabPoint != null)
                {
                    //position = grabbable.grabPoint.position;
                    Position = PalmPosTarget.position;
                }
                else
                {
                    Position = PalmPosTarget.position;
                }

                if (Grabbable.Rb)
                {
                    Grabbable.Rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

                    if (Grabbable.GrabPoint)
                    {
                        Vector3 OriginalPosition = Grabbable.transform.position;

                        // Calculate and apply rotation.
                        Quaternion RotationDifference = transform.rotation * Quaternion.Inverse(Grabbable.GrabPoint.rotation);
                        Grabbable.transform.rotation = RotationDifference * Grabbable.transform.rotation;

                        // Calculate the offset from the grab point to the object's center.
                        Vector3 GrabOffset = Grabbable.transform.position - Grabbable.GrabPoint.position;

                        Vector3 NewPosition = Position + transform.rotation * Quaternion.Inverse(Grabbable.GrabPoint.rotation) * GrabOffset;

                        Grabbable.transform.position = NewPosition;

                        FixedJoint.connectedBody = Grabbable.Rb;
                        FixedJoint.connectedAnchor = Grabbable.Rb.transform.InverseTransformPoint(Position);

                        //Uncomment this when it's not done in fixedupdate (MovementAndRotation)
                        //Quaternion rot = Quaternion.FromToRotation(grabbable.grabPoint.transform.forward, transform.forward);
                        //palmRot = rot * palmPos.rotation;
                    }
                    else
                    {
                        FixedJoint.connectedBody = Grabbable.Rb;
                        FixedJoint.connectedAnchor = Grabbable.Rb.transform.InverseTransformPoint(Position);
                    }
                }
                else
                {
                    FixedJoint.connectedAnchor = Position;
                }
            }
        }
        //Releasing
        else if (!IsGrabButtonPressed && CurrentGrabbable)
        {
            StartCoroutine(SetColliders(true, ColliderEnableDelay));
            ForceRelease();
        }
    }

    private void MovementAndRotation()
    {
        //Moves the physics hands towards the controllers position.
        Rb.velocity = (Target.position - transform.position) / Time.fixedDeltaTime;

        //Rotates the physics hands towards the controllers rotation.
        Quaternion rotationDifference = Target.rotation * Quaternion.Inverse(transform.rotation);
        rotationDifference.ToAngleAxis(out float Angle, out Vector3 Axis);

        Vector3 rotation = Angle * Axis;

        Rb.angularVelocity = (rotation * Mathf.Deg2Rad) / Time.fixedDeltaTime;
    }

    private void MovePalm()
    {
        //Move palmpos to it's target.
        PalmPos.position = PalmPosTarget.position;

        //Rotate palmpos to it's target.
        if (CurrentGrabbable && CurrentGrabbable.GrabPoint)
        {
            //This script never got fully finished so some things didn't get fully optimized.
            //TODO: optimize this by calculating an offset and using localrotation instead.
            Quaternion Rot = Quaternion.FromToRotation(CurrentGrabbable.GrabPoint.transform.forward, transform.forward);
            Quaternion WorldRot = Rot * PalmPos.rotation;

            PalmPos.rotation = WorldRot;
        }
        else
        {
            PalmPos.localRotation = Quaternion.identity;
        }
    }

    /// <summary>
    /// Releases any object the hand is holding immediatly.
    /// </summary>
    public void ForceRelease()
    {
        //Change the collisionDectionMode of the Rigidbody to save performance.
        CurrentGrabbable.Rb.collisionDetectionMode = CollisionDetectionMode.Discrete;
        CurrentGrabbable.IsGrabbed = false;

        CurrentGrabbable.Hand = null;
        CurrentGrabbable = null;

        MayGrab = false;

        //Destroys the joint connecting the hand to the object
        if (FixedJoint) Destroy(FixedJoint);
    }

    private void OnDrawGizmosSelected()
    {
        //Shows the sphere where the object needs to be in order to be grabbed.
        if (PalmPosTarget) Gizmos.DrawWireSphere(PalmPosTarget.position, GrabCheckSize);
    }
}
