using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using System.Collections;

public class Movement : MonoBehaviour
{
    [Header("Control Mode")]
    [SerializeField] private bool useGamepad = false;

    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float groundCheckDistance = 1.1f;
    [SerializeField] private LayerMask groundLayer;
    [Header("Advanced Movement Tuning")]
    [SerializeField] private float acceleration = 15f;
    [SerializeField] private float airControlMultiplier = 0.5f;
    [SerializeField] private float debuffMultiplier;
    [SerializeField] private float debuffuration;

    [Header("Look Settings")]
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private bool invertY = false;
    [SerializeField] private float rotationSpeed = 100f;
    [SerializeField] private float pitchClamp = 80f;
    private PlayerInputActions inputActions;
    private Rigidbody rb;
    private float cameraPitch = 0f;
    private float originalMoveSpeed;
    private Coroutine debuffCoroutine;

    private void Awake()
    {
        inputActions = new PlayerInputActions();
        rb = GetComponent<Rigidbody>();

        if (useGamepad)
        {
            inputActions.Gamepad.Enable();
            inputActions.Keyboard.Disable();
        }
        else
        {
            inputActions.Keyboard.Enable();
            inputActions.Gamepad.Disable();
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        HandleRotation();

        bool jumpPressed = useGamepad
            ? inputActions.Gamepad.Jump.triggered
            : inputActions.Keyboard.Jump.triggered;

        if (IsGrounded() && jumpPressed)
        {
            Jump();
        }
    }

    private void FixedUpdate()
    {
        Vector2 input = useGamepad
            ? inputActions.Gamepad.Move.ReadValue<Vector2>()
            : inputActions.Keyboard.Move.ReadValue<Vector2>();

        Vector3 moveDirection = transform.forward * input.y + transform.right * input.x;
        moveDirection.Normalize();

        bool grounded = IsGrounded();
        float control = grounded ? 1f : airControlMultiplier;

        Vector3 desiredVelocity = moveDirection * moveSpeed;
        Vector3 currentVelocity = rb.velocity;
        Vector3 horizontalVelocity = new Vector3(currentVelocity.x, 0f, currentVelocity.z);

        Vector3 velocityChange = (desiredVelocity - horizontalVelocity) * acceleration * control;

        rb.AddForce(new Vector3(velocityChange.x, 0f, velocityChange.z), ForceMode.Force);

    }



    private void HandleRotation()
    {
        Vector2 lookInput = useGamepad
            ? inputActions.Gamepad.Look.ReadValue<Vector2>()
            : inputActions.Keyboard.Look.ReadValue<Vector2>();

        float x = lookInput.x * rotationSpeed * Time.deltaTime;
        float y = lookInput.y * rotationSpeed * Time.deltaTime * (invertY ? 1 : -1);

        transform.Rotate(Vector3.up * x);

        cameraPitch += y;
        cameraPitch = Mathf.Clamp(cameraPitch, -pitchClamp, pitchClamp);
        if (cameraTransform != null)
        {
            cameraTransform.localEulerAngles = new Vector3(cameraPitch, 0f, 0f);
        }
    }

    private void Jump()
    {
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    private bool IsGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, groundCheckDistance, groundLayer);
    }

    public void ApplySpeedDebuff()
    {
        if (debuffCoroutine != null)
        {
            StopCoroutine(debuffCoroutine);
        }
        debuffCoroutine = StartCoroutine(SpeedDebuffRoutine(debuffMultiplier, debuffuration));
    }

    private IEnumerator SpeedDebuffRoutine(float debuffMultiplier, float duration)
    {
        originalMoveSpeed = moveSpeed;
        moveSpeed *= debuffMultiplier;
        yield return new WaitForSeconds(duration);
        moveSpeed = originalMoveSpeed;
        debuffCoroutine = null;
    }

}
