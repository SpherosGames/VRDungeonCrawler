using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public Transform camera;
    public Transform player;
    public Collider capsuleCollider;




    
   
    private float walkingSpeed = 7.5f;
    private float runningSpeed = 11.5f;
    private float jumpSpeed = 8f;
    private float crouchJumpSpeed = 5f;
    private float crouchSpeed = 3.5f;
    private float lookSpeed = 2f;

    private float gravity = 20f;
    private float lookXLimit = 75f;
    private float crouchHeight = 0.5f;
    private float rotationX = 0f;

    private bool isCrouching = false;
    public bool canMove = true;
    public bool canMoveCamera = true;

    [HideInInspector]public CharacterController characterController;
    private Vector3 moveDirection = Vector3.zero;

    private bool isZoomed = false;
    private float zoomFOV = 18f; 
    private float normalFOV; 
    public float zoomSmoothTime = 0.2f; 
    private Coroutine zoomCoroutine;

    void Start()
    {
        capsuleCollider = GetComponentInChildren<CapsuleCollider>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        characterController = GetComponent<CharacterController>();

        normalFOV = Camera.main.fieldOfView;
    }

    void Update()
    {
        if (canMoveCamera)
        {
            rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
            rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
            Camera.main.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);

            transform.Rotate(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
        }

        if (canMove)
        {
         
            Vector3 distanceToPlayer = player.position - transform.position;

            if (!isCrouching)
            {
                Vector3 forward = transform.TransformDirection(Vector3.forward);
                Vector3 right = transform.TransformDirection(Vector3.right);
                bool isRunning = Input.GetKey(KeyCode.LeftShift);
                float curSpeedX = canMove ? (isRunning ? runningSpeed : walkingSpeed) * Input.GetAxis("Vertical") : 0;
                float curSpeedY = canMove ? (isRunning ? runningSpeed : walkingSpeed) * Input.GetAxis("Horizontal") : 0;
                float movementDirectionY = moveDirection.y;
                moveDirection = (forward * curSpeedX) + (right * curSpeedY);

                if (Input.GetButton("Jump") && canMove && characterController.isGrounded)
                    moveDirection.y = jumpSpeed;
                else
                    moveDirection.y = movementDirectionY;

                characterController.height = 2f;
                characterController.Move(moveDirection * Time.deltaTime);
            }
            else
            {
                Vector3 forward = transform.TransformDirection(Vector3.forward);
                Vector3 right = transform.TransformDirection(Vector3.right);
                float curSpeedX = crouchSpeed * Input.GetAxis("Vertical");
                float curSpeedY = crouchSpeed * Input.GetAxis("Horizontal");
                float movementDirectionY = moveDirection.y;
                moveDirection = (forward * curSpeedX) + (right * curSpeedY);

                if (Input.GetButton("Jump") && canMove && characterController.isGrounded)
                    moveDirection.y = crouchJumpSpeed;
                else
                    moveDirection.y = movementDirectionY;

                characterController.height = crouchHeight * 2;
                characterController.Move(moveDirection * Time.deltaTime);
            }
        }

        if (Input.GetKeyDown(KeyCode.C))
            isCrouching = true;

        if (Input.GetKeyUp(KeyCode.C))
            isCrouching = false;

        if (!characterController.isGrounded)
            moveDirection.y -= gravity * Time.deltaTime;

        if (Input.GetMouseButtonDown(1)) // Right mouse button
        {
            ToggleZoom();
        }
    }
    void ToggleZoom()
    {
        isZoomed = !isZoomed; // Toggle zoom state

        if (isZoomed)
        {
            // Zoom in
            if (zoomCoroutine != null)
                StopCoroutine(zoomCoroutine);

            zoomCoroutine = StartCoroutine(SmoothZoom(zoomFOV));
        }
        else
        {
            // Zoom out
            if (zoomCoroutine != null)
                StopCoroutine(zoomCoroutine);

            zoomCoroutine = StartCoroutine(SmoothZoom(normalFOV));
        }
    }
    IEnumerator SmoothZoom(float targetFOV)
    {
        float currentFOV = Camera.main.fieldOfView;
        float velocity = 0f;

        while (Mathf.Abs(currentFOV - targetFOV) > 0.01f)
        {
            currentFOV = Mathf.SmoothDamp(currentFOV, targetFOV, ref velocity, zoomSmoothTime);
            Camera.main.fieldOfView = currentFOV;
            yield return null;
        }

        Camera.main.fieldOfView = targetFOV;
    }
}
