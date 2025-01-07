using UnityEngine;

public class FollowObject : MonoBehaviour
{
    [SerializeField] private Transform Target;

    [SerializeField] private Vector3 PosOffset;
    [SerializeField] private bool FollowPos;
    [SerializeField] private bool FollowPosX;
    [SerializeField] private bool FollowPosY;
    [SerializeField] private bool FollowPosZ;

    [SerializeField] private Vector3 RotOffset;
    [SerializeField] private bool FollowRot;
    [SerializeField] private bool FollowRotX;
    [SerializeField] private bool FollowRotY;
    [SerializeField] private bool FollowRotZ;

    private void Update()
    {
        //Follow the position of the specified axis.
        if (FollowPos)
        {
            Vector3 FinalPos = Target.position + PosOffset;

            if (!FollowPosX) FinalPos.x = transform.position.x;
            if (!FollowPosY) FinalPos.y = transform.position.y;
            if (!FollowPosZ) FinalPos.z = transform.position.z;

            transform.position = FinalPos;
        }

        //Follow the rotation of the specified axis.
        if (FollowRot)
        {
            Vector3 TargetRotation = Target.transform.rotation.eulerAngles;

            if (!FollowRotX) TargetRotation.x = transform.rotation.x;
            if (!FollowRotY) TargetRotation.y = transform.rotation.y;
            if (!FollowRotZ) TargetRotation.z = transform.rotation.z;

            Quaternion Rotation = Quaternion.Euler(TargetRotation) * Quaternion.Euler(RotOffset);

            transform.rotation = Rotation;

            //Set the position so that it rotates around the middle point.
            Vector3 NewPos = Rotation * (transform.position - Target.position) + Target.position;

            transform.position = NewPos;
        }
    }
}
