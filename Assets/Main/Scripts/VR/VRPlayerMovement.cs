using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class VRPlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed;
    [SerializeField] private float turnSpeed;
    [SerializeField] private bool snapTurn;
    [SerializeField] private float snapTurnAngle;
    [SerializeField] private float snapTurnCooldown;
    [SerializeField] private InputActionProperty moveInput;
    [SerializeField] private InputActionProperty turnInput;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Transform directionSource;
    [SerializeField] private Transform pivotPoint;

    private Vector2 moveInputAxis;
    private float inputTurnAxis;

    private float snapTurnTimer;

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
        Quaternion yaw = Quaternion.Euler(0, directionSource.eulerAngles.y, 0);

        Vector3 dir = Vector3.zero;

        if (moveInputAxis != Vector2.zero)
        {
            dir = yaw * new Vector3(moveInputAxis.x, 0, moveInputAxis.y);
        }

        rb.MovePosition(rb.position + moveSpeed * Time.fixedDeltaTime * dir);

        if (snapTurn)
        {
            if (snapTurnTimer <= 0)
            {
                if (inputTurnAxis > 0.5f) SnapTurn(snapTurnAngle);
                else if (inputTurnAxis < -0.5f) SnapTurn(-snapTurnAngle);
            }

            return;
        }

        //Smooth rotation
        Quaternion rotation = Quaternion.AngleAxis(turnSpeed * Time.fixedDeltaTime * inputTurnAxis, Vector3.up);

        rb.MoveRotation(rb.rotation * rotation);

        //Correct position so that the player can move away from middle of the playspace
        Vector3 newPos = rotation * (rb.position - pivotPoint.position) + pivotPoint.position;

        rb.MovePosition(newPos);
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
