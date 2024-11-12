using UnityEngine;

public class FollowObject : MonoBehaviour
{
    [SerializeField] private Transform target;

    [SerializeField] private Vector3 posOffset;
    [SerializeField] private bool followPos;
    [SerializeField] private bool followPosX;
    [SerializeField] private bool followPosY;
    [SerializeField] private bool followPosZ;

    [SerializeField] private Vector3 rotOffset;
    [SerializeField] private bool followRot;
    [SerializeField] private bool followRotX;
    [SerializeField] private bool followRotY;
    [SerializeField] private bool followRotZ;

    private void Update()
    {
        if (followPos)
        {
            Vector3 finalPos = target.position + posOffset;

            if (!followPosX) finalPos.x = transform.position.x;
            if (!followPosY) finalPos.y = transform.position.y;
            if (!followPosZ) finalPos.z = transform.position.z;

            transform.position = finalPos;
        }

        if (followRot)
        {
            Vector3 targetRotation = target.transform.rotation.eulerAngles;

            if (!followRotX) targetRotation.x = transform.rotation.x;
            if (!followRotY) targetRotation.y = transform.rotation.y;
            if (!followRotZ) targetRotation.z = transform.rotation.z;

            Quaternion rotation = Quaternion.Euler(targetRotation) * Quaternion.Euler(rotOffset);

            transform.rotation = rotation;

            Vector3 newPos = rotation * (transform.position - target.position) + target.position;

            transform.position = newPos;
        }
    }
}
