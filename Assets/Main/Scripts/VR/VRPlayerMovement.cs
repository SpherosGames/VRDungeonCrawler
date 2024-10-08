using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class VRPlayerMovement : MonoBehaviour
{
    [SerializeField] private float walkSpeed;
    [SerializeField] private float sprintSpeed;
    [SerializeField] private InputActionProperty sprintButton;
    [SerializeField] private float turnSpeed;
    [SerializeField] private bool snapTurn;
    [SerializeField] private float snapTurnAngle;
    [SerializeField] private float snapTurnCooldown;
    [SerializeField] private InputActionProperty moveInput;
    [SerializeField] private InputActionProperty turnInput;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Transform directionSource;
    [SerializeField] private Transform pivotPoint;

    [Header("Body Collider")]
    [SerializeField] private Transform playerHead;
    [SerializeField] private CapsuleCollider bodyCollider;
    [SerializeField] private Vector2 bodyHeightLimits = new Vector2(0.5f, 2f);

    private Vector2 moveInputAxis;
    private float inputTurnAxis;

    private float snapTurnTimer;

    private float moveSpeed;

    private void Awake()
    {
        if (!rb) rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        moveInputAxis = moveInput.action.ReadValue<Vector2>();
        inputTurnAxis = turnInput.action.ReadValue<Vector2>().x;

        if (snapTurn) snapTurnTimer -= Time.deltaTime;
    }

    private void FixedUpdate()
    {
        bodyCollider.height = Mathf.Clamp(playerHead.position.y, bodyHeightLimits.x, bodyHeightLimits.y);
        bodyCollider.center = new Vector3(playerHead.localPosition.x, bodyCollider.height / 2, playerHead.localPosition.z);

        Sprinting();

        Movement();

        SnapTurn();

        if (snapTurn) return;

        SmoothRotation();
    }

    private void Sprinting()
    {
        bool isSprinting = sprintButton.action.ReadValue<float>() > 0.5f;

        moveSpeed = isSprinting ? sprintSpeed : walkSpeed;
    }

    private void SmoothRotation()
    {
        //Smooth rotation
        Quaternion rotation = Quaternion.AngleAxis(turnSpeed * Time.fixedDeltaTime * inputTurnAxis, Vector3.up);

        rb.MoveRotation(rb.rotation * rotation);

        //Correct position so that the player can move away from middle of the playspace
        Vector3 newPos = rotation * (rb.position - pivotPoint.position) + pivotPoint.position;

        rb.MovePosition(newPos);
    }

    private void SnapTurn()
    {
        if (snapTurn)
        {
            if (snapTurnTimer <= 0)
            {
                if (inputTurnAxis > 0.5f) SnapTurn(snapTurnAngle);
                else if (inputTurnAxis < -0.5f) SnapTurn(-snapTurnAngle);
            }
        }
    }

    private void Movement()
    {
        Quaternion yaw = Quaternion.Euler(0, directionSource.eulerAngles.y, 0);

        Vector3 dir = Vector3.zero;

        if (moveInputAxis != Vector2.zero)
        {
            dir = yaw * new Vector3(moveInputAxis.x, 0, moveInputAxis.y);
        }

        rb.MovePosition(rb.position + moveSpeed * Time.fixedDeltaTime * dir);
    }

    private void SnapTurn(float angle)
    {
        Quaternion newRotation = Quaternion.Euler(0, angle, 0);

        rb.MoveRotation(rb.rotation * newRotation);

        //Correct position so that the player can move away from middle of the playspace
        Vector3 newPos = newRotation * (rb.position - pivotPoint.position) + pivotPoint.position;

        rb.MovePosition(newPos);

        snapTurnTimer = snapTurnCooldown;
    }
}
