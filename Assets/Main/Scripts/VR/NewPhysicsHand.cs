using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class NewPhysicsHand : MonoBehaviour
{
    [SerializeField] private Transform Target;
    [SerializeField] private Rigidbody Rb;
    [SerializeField] private PhysicMaterial HandMaterial;
    [SerializeField] private Transform PalmPos;
    [SerializeField] private Vector3 PositionOffset;
    [SerializeField] private Vector3 RotationOffset;
    [SerializeField] private float FollowSpeed;
    [SerializeField] private float RotationSpeed;
    [SerializeField] private int NonInteractLayer;
   
    private void OnEnable()
    {
        //Set rigidbody.
        if (Rb == null) Rb = GetComponent<Rigidbody>();

        //Teleport the physics hands towards the controllers so that they start at the correct position.
        //Otherwise the hands can get stuck somewhere trying to get to the controllers.
        transform.position = Target.transform.position;
    }

    private void FixedUpdate()
    {
        MovementAndRotation();
    }

    private void MovementAndRotation()
    {
        //Move the physics hands towards the controllers with "Rb.velocity" so that the physics hands can collide with the world.
        var PositionWithOffset = Target.TransformPoint(PositionOffset);
        var Distance = Vector3.Distance(PositionWithOffset, transform.position);
        Rb.velocity = (PositionWithOffset - transform.position).normalized * FollowSpeed * Distance * Time.deltaTime;

        //Rotate the physics hand towards the controllers with "Rb.angularVelocity" so that the physics hands can collide with the world.
        var RotationWithOffset = Target.rotation * Quaternion.Euler(RotationOffset);
        var Q = RotationWithOffset * Quaternion.Inverse(Rb.rotation);
        Q.ToAngleAxis(out float Angle, out Vector3 Axis);
        if (Mathf.Abs(Axis.magnitude) != Mathf.Infinity)
        {
            if (Angle > 180.0f) { Angle -= 360.0f; }
            Rb.angularVelocity = Angle * Mathf.Deg2Rad * RotationSpeed * Time.deltaTime * Axis;
        }
    }

    //Turn collision off so that the held object does not collide with the hand.
    public void TurnCollisionOff(SelectEnterEventArgs Args)
    {
        Args.interactableObject.transform.gameObject.layer = NonInteractLayer;
    }

    //Turn collision on so that the hand can collide with the world again.
    public void TurnCollisionOn(SelectExitEventArgs Args)
    {
        //Hardcoding the layer to the interactable layer.
        Args.interactableObject.transform.gameObject.layer = 10;
    }
}
