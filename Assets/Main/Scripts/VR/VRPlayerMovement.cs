using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Timeline;
using UnityEngine.XR.Interaction.Toolkit;

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

    [Header("Jumping")]
    [SerializeField] private float jumpForce;
    [SerializeField] private Transform feetPos;
    [SerializeField] private float groundCheckRadius = 0.1f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private InputActionProperty jumpButton;
    [SerializeField] private float minHandVelocity;
    [SerializeField] private Transform leftHand;
    [SerializeField] private Transform rightHand;

    [Header("Body Collider")]
    [SerializeField] private Transform playerHead;
    [SerializeField] private CapsuleCollider bodyCollider;
    [SerializeField] private Vector2 bodyHeightLimits = new Vector2(0.5f, 2f);

    private Vector2 moveInputAxis;
    private float inputTurnAxis;

    private bool sprintButtonState;

    private float snapTurnTimer;

    private float moveSpeed;

    private bool isGrounded;

    private float leftHandYVel;
    private float rightHandYVel;

    private Vector3 lastLeftHandPos;
    private Vector3 lastRightHandPos;

    private void Awake()
    {
        if (!rb) rb = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        jumpButton.action.performed += (InputAction.CallbackContext context) => Jumping();
    }

    private void Update()
    {
        moveInputAxis = moveInput.action.ReadValue<Vector2>();
        inputTurnAxis = turnInput.action.ReadValue<Vector2>().x;
        sprintButtonState = sprintButton.action.ReadValue<float>() > 0.5f;

        if (snapTurn) snapTurnTimer -= Time.deltaTime;

        CalculateHandVelocities();
    }

    private void CalculateHandVelocities()
    {
        leftHandYVel = (leftHand.position.y - lastLeftHandPos.y) / Time.deltaTime;
        rightHandYVel = (rightHand.position.y - lastRightHandPos.y) / Time.deltaTime;

        lastLeftHandPos = leftHand.position;
        lastRightHandPos = rightHand.position;
    }

    private void FixedUpdate()
    {
        Crouching();

        CheckGrounded();

        Sprinting();

        Movement();

        SnapTurn();

        if (snapTurn) return;

        SmoothRotation();
    }

    private void CheckGrounded()
    {
        isGrounded = Physics.CheckSphere(feetPos.position, groundCheckRadius, groundLayer);
    }

    private void Jumping()
    {
        bool handsJumping = false;

        if (leftHandYVel > minHandVelocity && rightHandYVel > minHandVelocity)
        {
            handsJumping = true;
        }

        if (handsJumping && isGrounded)
        {
            float extraJumpAmount = Mathf.Clamp(((leftHandYVel + rightHandYVel) / 4), 1, 2);

            rb.AddForce(Vector3.up * (jumpForce * extraJumpAmount), ForceMode.Impulse);
        }
    }

    private void Crouching()
    {
        bodyCollider.height = Mathf.Clamp(playerHead.position.y, bodyHeightLimits.x, bodyHeightLimits.y);
        bodyCollider.center = new Vector3(playerHead.localPosition.x, bodyCollider.height / 2, playerHead.localPosition.z);
    }

    private void Sprinting()
    {
        moveSpeed = sprintButtonState ? sprintSpeed : walkSpeed;
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

    Vector3 moveDir;

    private void Movement()
    {
        Quaternion yaw = Quaternion.Euler(0, directionSource.eulerAngles.y, 0);

        if (moveInputAxis != Vector2.zero && isGrounded)
        {
            moveDir = yaw * new Vector3(moveInputAxis.x, 0, moveInputAxis.y);
        }
        else if (moveInputAxis == Vector2.zero && isGrounded)
        {
            moveDir = Vector3.zero;
        }

        rb.MovePosition(rb.position + moveSpeed * Time.fixedDeltaTime * moveDir);
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

    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(feetPos.position, groundCheckRadius);
    }
}
