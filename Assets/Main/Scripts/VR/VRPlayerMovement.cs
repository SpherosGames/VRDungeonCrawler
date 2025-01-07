using UnityEngine;
using UnityEngine.InputSystem;

public class VRPlayerMovement : MonoBehaviour
{
    [SerializeField] private float WalkSpeed;
    [SerializeField] private float SprintSpeed;
    [SerializeField] private InputActionProperty SprintButton;
    [SerializeField] private float TurnSpeed;
    [SerializeField] private bool UseSnapTurn;
    [SerializeField] private float SnapTurnAngle;
    [SerializeField] private float SnapTurnCooldown;
    [SerializeField] private InputActionProperty MoveInput;
    [SerializeField] private InputActionProperty TurnInput;
    [SerializeField] private Rigidbody Rb;
    [SerializeField] private Transform DirectionSource;
    [SerializeField] private Transform PivotPoint;
    [SerializeField] private float ExtraGravity = 50;

    [Header("Jumping")]
    [SerializeField] private float JumpForce;
    [SerializeField] private Transform FeetPos;
    [SerializeField] private float GroundCheckRadius = 0.1f;
    [SerializeField] private LayerMask GroundLayer;
    [SerializeField] private InputActionProperty JumpButton;
    [SerializeField] private float MinHandVelocity;
    [SerializeField] private Transform LeftHand;
    [SerializeField] private Transform RightHand;

    [Header("Body Collider")]
    [SerializeField] private Transform PlayerHead;
    [SerializeField] private CapsuleCollider BodyCollider;
    [SerializeField] private Vector2 BodyHeightLimits = new(0.5f, 2f);

    private Vector2 MoveInputAxis;
    private float InputTurnAxis;

    private bool SprintButtonState;

    private float SnapTurnTimer;

    private float MoveSpeed;

    private bool IsGrounded;

    private float LeftHandYVel;
    private float RightHandYVel;

    private Vector3 LastLeftHandPos;
    private Vector3 LastRightHandPos;

    public bool MayMove;
    public bool MayTurn;

    private void Awake()
    {
        //Assign Rigidbody.
        if (!Rb) Rb = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        //Call jump function when the jump button is pressed.
        JumpButton.action.performed += (InputAction.CallbackContext context) => Jumping();

        MayMove = true;
        MayTurn = true;
    }

    private void Update()
    {
        //Set input variables.
        MoveInputAxis = MoveInput.action.ReadValue<Vector2>();
        InputTurnAxis = TurnInput.action.ReadValue<Vector2>().x;
        SprintButtonState = SprintButton.action.ReadValue<float>() > 0.5f;

        if (UseSnapTurn) SnapTurnTimer -= Time.deltaTime;

        CalculateHandVelocities();
    }

    //Calculate the speed of both hands, for jumping.
    private void CalculateHandVelocities()
    {
        LeftHandYVel = (LeftHand.position.y - LastLeftHandPos.y) / Time.deltaTime;
        RightHandYVel = (RightHand.position.y - LastRightHandPos.y) / Time.deltaTime;

        LastLeftHandPos = LeftHand.position;
        LastRightHandPos = RightHand.position;
    }

    private void FixedUpdate()
    {
        //Extra gravity.
        Rb.AddForce(ExtraGravity * Rb.mass * Vector3.down);

        Crouching();

        CheckGrounded();

        Sprinting();

        if (MayMove) Movement();

        //Use snap turn or smooth turn based on boolean.
        if (MayTurn)
        {
            if (UseSnapTurn) SnapTurn();
            else SmoothRotation();
        }
    }

    //Use checksphere to check if the feet are hitting the ground.
    private void CheckGrounded()
    {
        IsGrounded = Physics.CheckSphere(FeetPos.position, GroundCheckRadius, GroundLayer);
    }
    
    //If the hands are moving fast enough and the player is grounded: jump.
    private void Jumping()
    {
        bool handsJumping = false;

        if (LeftHandYVel > MinHandVelocity && RightHandYVel > MinHandVelocity)
        {
            handsJumping = true;
        }

        if (handsJumping && IsGrounded)
        {
            //Jump higher based on speed of hands with limit.
            float extraJumpAmount = Mathf.Clamp(((LeftHandYVel + RightHandYVel) / 4), 1, 2);

            Rb.AddForce(Vector3.up * (JumpForce * extraJumpAmount), ForceMode.Impulse);
        }
    }

    //Crouch by changing the body collider (capsule collider) based on head position with limits.
    private void Crouching()
    {
        BodyCollider.height = Mathf.Clamp(PlayerHead.position.y, BodyHeightLimits.x, BodyHeightLimits.y);
        BodyCollider.center = new Vector3(PlayerHead.localPosition.x, BodyCollider.height / 2, PlayerHead.localPosition.z);
    }

    //Change "MoveSpeed" based on if the sprint button is pressed or not.
    private void Sprinting()
    {
        MoveSpeed = SprintButtonState ? SprintSpeed : WalkSpeed;
    }

    private void SmoothRotation()
    {
        Quaternion rotation = Quaternion.AngleAxis(TurnSpeed * Time.fixedDeltaTime * InputTurnAxis, Vector3.up);

        Rb.MoveRotation(Rb.rotation * rotation);

        //Correct position so that the player can move away from middle of the playspace.
        Vector3 newPos = rotation * (Rb.position - PivotPoint.position) + PivotPoint.position;

        Rb.MovePosition(newPos);
    }

    private void SnapTurn()
    {
        //Only allow snap turn every "SnapTurnCooldown" seconds.
        if (SnapTurnTimer <= 0)
        {
            if (InputTurnAxis > 0.5f) SnapTurn(SnapTurnAngle);
            else if (InputTurnAxis < -0.5f) SnapTurn(-SnapTurnAngle);
        }
    }

    Vector3 MoveDir;

    private void Movement()
    {
        Quaternion yaw = Quaternion.Euler(0, DirectionSource.eulerAngles.y, 0);
        
        if (IsGrounded)
        {
            //If the player is inputting a value for movement, calculate the movement direction.
            if (MoveInputAxis != Vector2.zero)
            {
                MoveDir = yaw * new Vector3(MoveInputAxis.x, 0, MoveInputAxis.y);
            }
            //If the player is not inputting a value for movement, reset the movement direction.
            //This is so that the player stops moving when no values are inputted, but not when airborne.
            else if (MoveInputAxis == Vector2.zero)
            {
                MoveDir = Vector3.zero;
            }
        }

        Rb.velocity = MoveSpeed * MoveDir;
    }

    private void SnapTurn(float angle)
    {
        Quaternion newRotation = Quaternion.Euler(0, angle, 0);

        Rb.MoveRotation(Rb.rotation * newRotation);

        //Correct position so that the player can move away from middle of the playspace.
        Vector3 newPos = newRotation * (Rb.position - PivotPoint.position) + PivotPoint.position;

        Rb.MovePosition(newPos);

        SnapTurnTimer = SnapTurnCooldown;
    }

    private void OnDrawGizmos()
    {
        //Draw a sphere at the players feet for debugging.
        Gizmos.DrawSphere(FeetPos.position, GroundCheckRadius);
    }
}
