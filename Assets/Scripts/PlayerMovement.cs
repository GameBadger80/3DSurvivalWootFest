using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("References")]
    public Camera playerCamera;
    public Animator animator;
    public Transform visualRoot; // visual parent (no rotation yet)

    [Header("Movement")]
    public float walkSpeed = 2f;
    public float runSpeed = 4f;
    public float crouchSpeed = 1f;
    public float jumpPower = 4f;
    public float gravity = 10f;

    [Header("Crouch")]
    public float defaultHeight = 2f;
    public float crouchHeight = 1f;

    [Header("Mouse Look")]
    public float lookSpeed = 2f;
    public float lookXLimit = 45f;
    public bool invertLookY = false;

    [Header("Camera Crouch")]
    public float standingCameraHeight = 1.6f;
    public float crouchCameraHeight = 1.0f;
    public float cameraCrouchSpeed = 8f;
    private Vector3 cameraLocalPos;


    private Vector3 moveDirection;
    private float rotationX;
    private float currentSpeed;
    private CharacterController characterController;
    private bool canMove = true;

    void Start()
    {
        cameraLocalPos = playerCamera.transform.localPosition;
        characterController = GetComponent<CharacterController>();

        if (!animator)
            animator = GetComponent<Animator>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        HandleCameraCrouch();
        HandleMovement();
        HandleMouseLook();
        HandleAnimations();

        // Gather test
        if (Input.GetKeyDown(KeyCode.E))
        {
            animator.SetTrigger("GatherTrigger");
        }
    }

    // ---------------- MOVEMENT ----------------

    void HandleMovement()
    {
        bool isRunning = Input.GetKey(KeyCode.LeftShift);
        bool isCrouching = Input.GetKey(KeyCode.LeftControl);

        // Stop movement during gather BUT still apply gravity
        if (animator.GetCurrentAnimatorStateInfo(0).IsTag("Gather"))
        {
            moveDirection.y -= gravity * Time.deltaTime;
            characterController.Move(moveDirection * Time.deltaTime);
            return;
        }

        Vector3 forward = transform.forward;
        Vector3 right = transform.right;

        // Speed + capsule
        if (isCrouching)
        {
            currentSpeed = crouchSpeed;
            characterController.height = crouchHeight;
            characterController.center = new Vector3(0, crouchHeight / 2f, 0);
        }
        else
        {
            currentSpeed = isRunning ? runSpeed : walkSpeed;
            characterController.height = defaultHeight;
            characterController.center = new Vector3(0, defaultHeight / 2f, 0);
        }

        // Input
        float inputX = Input.GetAxis("Vertical");   // W / S
        float inputY = Input.GetAxis("Horizontal"); // A / D

        Vector3 move = (forward * inputX + right * inputY).normalized * currentSpeed;

        // Horizontal movement
        moveDirection.x = move.x;
        moveDirection.z = move.z;

        // Ground / air logic
        if (characterController.isGrounded)
        {
            if (moveDirection.y < 0f)
                moveDirection.y = -10f; // snap to ground

            if (Input.GetButtonDown("Jump") && !isCrouching)
            {
                moveDirection.y = jumpPower;
                animator.SetTrigger("JumpTrigger");
            }
        }
        else
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }

        characterController.Move(moveDirection * Time.deltaTime);
    }

    // ---------------- CAMERA ----------------

    void HandleMouseLook()
    {
        if (!canMove) return;

        float mouseX = Input.GetAxis("Mouse X") * lookSpeed;
        float mouseY = Input.GetAxis("Mouse Y") * lookSpeed;

        rotationX += invertLookY ? mouseY : -mouseY;
        rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);

        playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }

    void HandleCameraCrouch()
    {
        bool isCrouching = Input.GetKey(KeyCode.LeftControl);

        float targetY = isCrouching ? crouchCameraHeight : standingCameraHeight;

        Vector3 targetPos = new Vector3(
            cameraLocalPos.x,
            targetY,
            cameraLocalPos.z
        );

        playerCamera.transform.localPosition = Vector3.Lerp(
            playerCamera.transform.localPosition,
            targetPos,
            Time.deltaTime * cameraCrouchSpeed
        );
    }

    // ---------------- ANIMATIONS ----------------

    void HandleAnimations()
    {
        if (!animator) return;

        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        bool isRunning = Input.GetKey(KeyCode.LeftShift);
        bool isCrouching = Input.GetKey(KeyCode.LeftControl);

        float speedMultiplier = isRunning ? 2f : 1f;

        animator.SetFloat("Horizontal", h * speedMultiplier, 0.1f, Time.deltaTime);
        animator.SetFloat("Vertical", v * speedMultiplier, 0.1f, Time.deltaTime);

        
        animator.SetBool("IsGrounded", characterController.isGrounded);
        animator.SetFloat("VerticalVelocity", moveDirection.y);

        int crouchLayer = animator.GetLayerIndex("CrouchLayer");
        animator.SetLayerWeight(crouchLayer, isCrouching ? 1f : 0f);


    }
}
