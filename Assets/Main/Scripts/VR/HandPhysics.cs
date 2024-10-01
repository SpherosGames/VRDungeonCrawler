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

    private ConfigurableJoint joint;

    private Grabbable currentGrabbable;

    private Grabbable socketedObject;

    private bool mayGrab = true;

    private Quaternion palmRot;

    private void Awake()
    {
        colliders = GetComponentsInChildren<Collider>();
        colliders.ToList().ForEach(x => x.material = handMaterial);
    }

    private void OnEnable()
    {
        //Set rigidbody
        if (rb == null) rb = GetComponent<Rigidbody>();

        ResetPalmRot();
    }

    private void ResetPalmRot()
    {
        palmRot = palmPosTarget.rotation;
        palmPos.rotation = palmRot;
    }

    private IEnumerator SetColliders(bool value, float delay)
    {
        yield return new WaitForSeconds(delay);

        if (value) print("Turn on");

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

                StartCoroutine(SetColliders(false, 0));

                if (!useJoint) return;

                //Setup fixedjoint
                joint = palmPos.gameObject.AddComponent<ConfigurableJoint>();
                joint.autoConfigureConnectedAnchor = false;
                joint.angularXMotion = ConfigurableJointMotion.Locked;
                joint.angularYMotion = ConfigurableJointMotion.Locked;
                joint.angularZMotion = ConfigurableJointMotion.Locked;
                joint.xMotion = ConfigurableJointMotion.Locked;
                joint.yMotion = ConfigurableJointMotion.Locked;
                joint.zMotion = ConfigurableJointMotion.Locked;

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
                    grabbable.rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

                    if (grabbable.grabPoint)
                    {
                        print("Grab point go brr");
                        joint.connectedBody = grabbable.GetComponent<Rigidbody>();
                        joint.connectedAnchor = grabbable.rb.transform.InverseTransformPoint(position);

                        Quaternion rot = Quaternion.FromToRotation(grabbable.grabPoint.transform.forward, transform.forward);
                        palmRot = rot * palmPos.rotation;
                    }
                    else
                    {
                        //fixedJoint.axis = transform.right;
                        joint.connectedBody = grabbable.rb;
                        joint.connectedAnchor = grabbable.rb.transform.InverseTransformPoint(position);
                    }
                }
                else
                {
                    joint.connectedAnchor = position;
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

    Vector3 velocity = Vector3.zero;

    private void MovementAndRotation()
    {
        //Phyics position
        rb.velocity = (target.position - transform.position) / Time.fixedDeltaTime;

        //Physics rotation
        Quaternion rotationDifference = target.rotation * Quaternion.Inverse(transform.rotation);
        rotationDifference.ToAngleAxis(out float angle, out Vector3 axis);

        Vector3 rotation = angle * axis;

        rb.angularVelocity = (rotation * Mathf.Deg2Rad) / Time.fixedDeltaTime;

        Rigidbody palmRB = palmPos.GetComponent<Rigidbody>();

        //Phyics position
        //Vector3 dir = palmPosTarget.position - palmPos.position;

        Vector3 desiredVelocity = CalculateVelocityToReachTarget(palmRB, palmPos.position, palmPosTarget.position);

        //print("Desired vel: " + desiredVelocity);

        //Vector3 targetVelo = (palmPosTarget.position - palmPos.position) / Time.fixedDeltaTime;

        //palmRB.velocity = Vector3.MoveTowards(palmRB.velocity, targetVelo, 10000);

        palmRB.velocity = desiredVelocity;

        if (currentGrabbable && currentGrabbable.grabPoint)
        {
            Quaternion rot = Quaternion.FromToRotation(currentGrabbable.grabPoint.transform.forward, transform.forward);
            palmRot = rot * palmPos.rotation;

            //palmRot = Quaternion.identity;
        }
        else
        {
            palmRot = Quaternion.identity;
            //Quaternion rot = Quaternion.FromToRotation(palmPos.forward, palmPosTarget.forward);
            //palmRot = rot * palmPos.rotation;
        }

        if (currentGrabbable && currentGrabbable.grabPoint)
        {
            //Quaternion rotationDifference2 = palmRot * Quaternion.Inverse(palmPos.rotation);
            //rotationDifference2.ToAngleAxis(out float angle2, out Vector3 axis2);

            //Vector3 rotation2 = angle2 * axis2;

            //palmRB.angularVelocity = (rotation2 * Mathf.Deg2Rad) / Time.fixedDeltaTime;

            //HSVR GO BRR
            //Vector3 targetVelo = (_handTargetTransform.position - _bdy.Position) / Time.fixedDeltaTime;
            //Vector3 targetRotVelo = HSVR.AngularVelocity(_bdy.Rotation, _handTargetTransform.rotation, Time.fixedDeltaTime);

            //_bdy.Velocity = Vector3.MoveTowards(_bdy.Velocity, targetVelo, HSVR.MAXVELOCITY);
            //_bdy.AngularVelocity = Vector3.MoveTowards(_bdy.AngularVelocity, targetRotVelo, HSVR.MAXANGULARVELOCITY);

            Vector3 targetRotVelo = AngularVelocity(palmRB.rotation, palmRot, Time.fixedDeltaTime);

            palmRB.angularVelocity = Vector3.MoveTowards(palmRB.angularVelocity, targetRotVelo, 1000);

            //palmPos.rotation = palmRot;
        }
        else
        {
            palmPos.localRotation = palmRot;
        }
    }

    public void ForceRelease()
    {
        print("Delete");
        ResetPalmRot();

        currentGrabbable.rb.collisionDetectionMode = CollisionDetectionMode.Discrete;

        currentGrabbable.hand = null;
        currentGrabbable = null;

        mayGrab = false;

        if (joint) Destroy(joint);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(palmPosTarget.position, grabCheckSize);
    }

    Vector3 CalculateVelocityToReachTarget(Rigidbody rb, Vector3 currentPosition, Vector3 targetPosition)
    {
        Vector3 displacement = targetPosition - currentPosition;

        // Calculate the velocity needed to cover the distance in one frame
        Vector3 requiredVelocity = displacement / Time.fixedDeltaTime;

        float totalMass = rb.mass + (currentGrabbable ? currentGrabbable.rb.mass : 0);

        // Calculate the force needed to achieve this velocity change
        Vector3 force = (requiredVelocity - rb.velocity) * totalMass / Time.fixedDeltaTime;

        // Limit the force to prevent extreme accelerations
        float maxForce = totalMass * 100f; // Adjust this value as needed
        if (force.magnitude > maxForce)
        {
            force = force.normalized * maxForce;
        }

        // Calculate the final velocity based on the limited force
        Vector3 finalVelocity = rb.velocity + (force / rb.mass) * Time.fixedDeltaTime;

        return finalVelocity;
    }

    Vector3 CalculateAngularVelocityToReachTarget(Rigidbody rb, Quaternion currentRotation, Quaternion targetRotation)
    {
        Quaternion rotationDifference = targetRotation * Quaternion.Inverse(currentRotation);
        //Quaternion rotationDifference = Quaternion.RotateTowards(currentRotation, targetRotation, 10000);
        rotationDifference.ToAngleAxis(out float angle, out Vector3 axis);

        print("Angle: " + angle);

        if (angle > 180f)
            angle -= 360f;

        Vector3 targetAngularVelocity = axis * (angle * Mathf.Deg2Rad / Time.fixedDeltaTime);

        float totalMass = rb.mass + (currentGrabbable ? currentGrabbable.rb.mass : 0);

        // Calculate the torque needed to achieve this angular velocity change
        Vector3 torque = (targetAngularVelocity - rb.angularVelocity) * totalMass / Time.fixedDeltaTime;

        print("Torque: " + torque);

        // Limit the torque to prevent extreme rotations
        float maxTorque = rb.mass * 10f; // Adjust this value as needed
        if (torque.magnitude > maxTorque)
        {
            print("Max torque reached");
            torque = torque.normalized * maxTorque;
        }

        // Calculate the final angular velocity based on the limited torque
        Vector3 finalAngularVelocity = rb.angularVelocity + (torque / rb.mass) * Time.fixedDeltaTime;

        print("ang vel: " + finalAngularVelocity);

        return finalAngularVelocity;
    }

    public static Vector3 AngularVelocity(Quaternion from, Quaternion to, float delta)
    {
        return AngularVelocity(to * Quaternion.Inverse(from), delta);
    }
    /// <summary> Calculates angular velocity from a rotational delta </summary>
    public static Vector3 AngularVelocity(Quaternion rotDelta, float delta)
    {
        Vector3 axis = Vector3.zero; float angle = 0; rotDelta.ToAngleAxis(out angle, out axis);
        if (angle > 180) angle -= 360;
        if (angle < -180) angle += 360; // probably not necessary
        angle = Mathf.Clamp(angle, -1000 * Mathf.Rad2Deg * delta, 1000 * Mathf.Rad2Deg * delta);
        // if( angle/delta >  MAXANGULARVELOCITY ) angle =  MAXANGULARVELOCITY *delta;
        // if( angle/delta < -MAXANGULARVELOCITY ) angle = -MAXANGULARVELOCITY *delta;
        Vector3 aVelo = axis.normalized * angle * Mathf.Deg2Rad / delta;
        if (float.IsNaN(aVelo.x) || float.IsNaN(aVelo.y) || float.IsNaN(aVelo.z)) return Vector3.zero;
        return aVelo;
    }
}
