using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using System.Linq;
using static UnityEngine.Rendering.DebugUI;
using UnityEngine.InputSystem;

public class HandPhysics : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private XRDirectInteractor interactor;
    [SerializeField] private PhysicMaterial handMaterial;
    [SerializeField] private float colliderEnableDelay = 1;
    [SerializeField] private InputActionProperty grabButton;
    [SerializeField] private float grabThreshold = 0.1f;
    [SerializeField] private Transform palmPos;
    [SerializeField] private float grabCheckSize = 0.1f;
    [SerializeField] private LayerMask grabLayer;

    private Collider[] colliders;

    private FixedJoint joint;

    private void Awake()
    {
        colliders = GetComponentsInChildren<Collider>();
        colliders.ToList().ForEach(x => x.material = handMaterial);
    }

    private void OnEnable()
    {
        //Set rigidbody
        if (rb == null) rb = GetComponent<Rigidbody>();

        interactor.selectEntered.RemoveAllListeners();
        interactor.selectEntered.AddListener((SelectEnterEventArgs args) => StartCoroutine(SetColliders(false, 0)));

        interactor.selectExited.RemoveAllListeners();
        interactor.selectExited.AddListener((SelectExitEventArgs args) => StartCoroutine(SetColliders(true, colliderEnableDelay)));
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

        if (isGrabButtonPressed)
        {
            Collider[] colliders = Physics.OverlapSphere(palmPos.position, grabCheckSize, grabLayer, QueryTriggerInteraction.Ignore);

            if (colliders.Length > 0)
            {
                Rigidbody rb = colliders[0].attachedRigidbody;
            }
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
