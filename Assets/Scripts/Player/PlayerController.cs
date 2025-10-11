using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private PlayerCannonController playerCannonController;
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private Rigidbody rb;

    [Header("Movement Stats")]
    [SerializeField] private float accelerationMaxSpeedRatio = 10.0f;
    [SerializeField] private float acceleration = 100.0f;
    [SerializeField] private float deceleration = 25.0f;
    [SerializeField] private float jumpStrength = 400.0f;
    [SerializeField] private float extraFallSpeed = 10.0f;
    [SerializeField] private float airControl = 1.0f;
    [SerializeField] private float rotationSpeed = 10.0f;

    [Header("Movement Powerup Stats")]
    private float baseJumpStrength;
    private Coroutine bananaPowerupRoutine;
    [SerializeField] private float bananaMult = 1.5f;
    [SerializeField] private float bananaDuration = 10.0f;
    private float baseAcceleration;
    private float baseDeceleration;
    private Coroutine hotdogPowerupRoutine;
    [SerializeField] private float hotdogMult = 1.5f;
    [SerializeField] private float hotdogDuration = 10.0f;

    private Vector3 maxVelocity;
    private Vector3 movementInput;

    private KeyCode forwardKey = KeyCode.W;
    private KeyCode leftKey = KeyCode.A;
    private KeyCode backKey = KeyCode.S;
    private KeyCode rightKey = KeyCode.D;

    private bool jumpTriggered = false;
    private KeyCode jumpKey = KeyCode.Space;

    private void Start()
    {
        UpdateMaxVelocity();
        if (extraFallSpeed < 1)
        {
            extraFallSpeed = 1;
        }

        baseJumpStrength = jumpStrength;
        baseAcceleration = acceleration;
        baseDeceleration = deceleration;
    }

    private void UpdateMaxVelocity()
    {
        maxVelocity = new Vector3(acceleration / accelerationMaxSpeedRatio, 0, acceleration / accelerationMaxSpeedRatio);
    }

    private void Update()
    {
        SetMovementInput();
        if (Input.GetKeyDown(jumpKey)) jumpTriggered = true;
    }

    private void FixedUpdate()
    {
        HorizontalMovement();
        RotateWithCamera();

        if (jumpTriggered)
        {
            Jump();
            jumpTriggered = false;
        }

        if (!IsGrounded())
            rb.AddForce(Vector3.down * extraFallSpeed, ForceMode.Impulse);
    }

    private void SetMovementInput()
    {
        movementInput = Vector3.zero;

        if (Input.GetKey(forwardKey)) movementInput += Vector3.forward;
        if (Input.GetKey(leftKey)) movementInput += Vector3.left;
        if (Input.GetKey(backKey)) movementInput += Vector3.back;
        if (Input.GetKey(rightKey)) movementInput += Vector3.right;

        movementInput = Vector3.ClampMagnitude(movementInput, 1);

        if (cameraTransform != null)
        {
            Vector3 fwd = Vector3.Scale(cameraTransform.forward, new Vector3(1, 0, 1).normalized);
            Vector3 right = cameraTransform.right;
            movementInput = movementInput.z * fwd + movementInput.x * right;
        }
    }

    private void RotateWithCamera()
    {
        if (movementInput.sqrMagnitude > 0 && cameraTransform != null || playerCannonController.GetIsAiming())
        {
            Vector3 fwd = Vector3.Scale(cameraTransform.forward, new Vector3(1, 0, 1)).normalized;
            Quaternion targetRotation = Quaternion.LookRotation(fwd);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
        }
    }

    private void HorizontalMovement()
    {
        Vector3 horizontalVelocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        Vector3 targetVelocity = movementInput * acceleration;

        if (IsGrounded())
        {
            Vector3 velocityChange = targetVelocity - horizontalVelocity;

            if (movementInput == Vector3.zero)
                velocityChange *= deceleration;

            rb.AddForce(velocityChange, ForceMode.Impulse);
        } else
        {
            Vector3 velocityChange = targetVelocity - horizontalVelocity;
            velocityChange *= airControl;
            rb.AddForce(velocityChange, ForceMode.Impulse);
        }

        if (rb.velocity.x > maxVelocity.x) rb.velocity = new Vector3(maxVelocity.x, rb.velocity.y, rb.velocity.z);
        if (rb.velocity.z > maxVelocity.z) rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y, maxVelocity.z);

        if (rb.velocity.x < -maxVelocity.x) rb.velocity = new Vector3(-maxVelocity.x, rb.velocity.y, rb.velocity.z);
        if (rb.velocity.z < -maxVelocity.z) rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y, -maxVelocity.z);
    }

    private bool IsGrounded()
    {
        CapsuleCollider collider = GetComponent<CapsuleCollider>();
        Vector3 origin = transform.position + Vector3.down * (collider.height / 2 - collider.radius);
        bool grounded = Physics.Raycast(origin, Vector3.down, 0.6f);
        return grounded;
    }

    private void Jump()
    {
        if (IsGrounded()) rb.AddForce(Vector3.up * jumpStrength, ForceMode.Impulse);
    }

    public void BananaPowerup()
    {
        if (bananaPowerupRoutine != null)
        {
            StopCoroutine(bananaPowerupRoutine);

            jumpStrength = baseJumpStrength;
        }

        bananaPowerupRoutine = StartCoroutine(BananaPowerupRoutine());
    }

    private IEnumerator BananaPowerupRoutine()
    {
        jumpStrength *= bananaMult;

        float timeLeft = bananaDuration;
        while (timeLeft > 0.0f)
        {
            timeLeft -= Time.deltaTime;
            yield return null;
        }

        jumpStrength = baseJumpStrength;
        bananaPowerupRoutine = null;
    }

    public void HotdogPowerup()
    {
        if (hotdogPowerupRoutine != null)
        {
            StopCoroutine(hotdogPowerupRoutine);

            acceleration = baseAcceleration;
            deceleration = baseDeceleration;
        }

        hotdogPowerupRoutine = StartCoroutine(HotdogPowerupRoutine());
    }

    private IEnumerator HotdogPowerupRoutine()
    {
        acceleration *= hotdogMult;
        deceleration *= hotdogMult;
        UpdateMaxVelocity();

        float timeLeft = hotdogDuration;
        while (timeLeft > 0.0f)
        {
            timeLeft -= Time.deltaTime;
            yield return null;
        }

        acceleration = baseAcceleration;
        deceleration = baseDeceleration;
        UpdateMaxVelocity();
        hotdogPowerupRoutine = null;
    }
}
