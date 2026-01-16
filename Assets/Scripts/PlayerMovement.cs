using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("References")]
    public Camera playerCamera;
    public Animator animator; // Animator for blend tree

    [Header("Movement")]
    public float walkSpeed = 6f;
    public float runSpeed = 12f;   // Can be added later
    public float crouchSpeed = 3f; // Can be added later
    public float jumpPower = 7f;
    public float gravity = 10f;

    [Header("Crouch")]
    public float defaultHeight = 2f;
    public float crouchHeight = 1f;

    [Header("Mouse Look")]
    public float lookSpeed = 2f;
    public float lookXLimit = 45f;
    public bool invertLookY = false;

    private Vector3 moveDirection = Vector3.zero;
    private float rotationX = 0f;
    private float currentSpeed;
    private CharacterController characterController;
    private bool canMove = true;

    void Start()
    {
        characterController = GetComponent<CharacterController>();

        if (animator == null)
            animator = GetComponent<Animator>(); // Safety check

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        HandleMovement();
        HandleMouseLook();
        HandleAnimations();
    }

    void HandleMovement()
    {
        bool isRunning = Input.GetKey(KeyCode.LeftShift);
        bool isCrouching = Input.GetKey(KeyCode.LeftControl);

        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);

        // Select speed
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
        float inputX = Input.GetAxis("Vertical");   // W/S keys
        float inputY = Input.GetAxis("Horizontal"); // A/D keys

        Vector3 move = (forward * inputX + right * inputY).normalized * currentSpeed;

        // Preserve vertical velocity
        float verticalVelocity = moveDirection.y;
        moveDirection = move;
        moveDirection.y = verticalVelocity;

        // Jump
        if (characterController.isGrounded)
        {
            if (moveDirection.y < 0)
                moveDirection.y = -2f; // Small downward force to stick to ground

            if (Input.GetButtonDown("Jump") && !isCrouching)
            {
                moveDirection.y = jumpPower;
                animator.SetTrigger("JumpTrigger"); // Trigger jump in JumpLayer
            }
        }

        // Gravity
        moveDirection.y -= gravity * Time.deltaTime;

        // Move the player
        characterController.Move(moveDirection * Time.deltaTime);
    }

    void HandleMouseLook()
    {
        if (!canMove) return;

        float mouseX = Input.GetAxis("Mouse X") * lookSpeed;
        float mouseY = Input.GetAxis("Mouse Y") * lookSpeed;

        rotationX += invertLookY ? mouseY : -mouseY;
        rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);

        playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0f, 0f);
        transform.rotation *= Quaternion.Euler(0f, mouseX, 0f);
    }

    void HandleAnimations()
    {
        if (animator == null)
            return;

        // Raw input
        float h = Input.GetAxis("Horizontal"); // A / D
        float v = Input.GetAxis("Vertical");   // W / S

        // Run key
        bool isRunning = Input.GetKey(KeyCode.LeftShift);

        

        // Walk = 1, Run = 2 (matches blend tree layout)
        float speedMultiplier = isRunning ? 2f : 1f;

        // Send values to Animator (smoothed)
        animator.SetFloat("Horizontal", h * speedMultiplier, 0.1f, Time.deltaTime);
        animator.SetFloat("Vertical", v * speedMultiplier, 0.1f, Time.deltaTime);
    }
}