using UnityEngine;

public class EnemyRagdollTest : MonoBehaviour
{
    [SerializeField] private ConfigurableJoint joint;
    [SerializeField] private Transform target;

    private void Update()
    {
        joint.targetRotation = target.rotation;
        joint.targetPosition = target.position;
    }
}
