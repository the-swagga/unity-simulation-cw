using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody rb;
    [SerializeField] private Transform look;
    [SerializeField] private ThirdPersonCamera cam;

    private float inputX;
    private float inputY;
    private Vector3 moveDirection;

    [Header("Movement Variables")]
    [SerializeField] private float baseSpeed;
    [SerializeField] private float maxSpeedMult;
    [SerializeField] private float groundDrag;
    [SerializeField] private float airDrag;
    [SerializeField] private float baseJumpStrength;
    [SerializeField] private float jumpCooldown;
    [SerializeField] private float airControl;
    private Vector3 horizontalVelocity = Vector3.zero;
    private float moveSpeed;
    private float maxSpeed;
    private float jumpStrength;

    [Header("Ground Check")]
    [SerializeField] private float playerHeight;
    [SerializeField] private LayerMask groundLayer;
    private bool canJump = true;
    private bool isGrounded;

    [Header("Powerup Variables")]
    [SerializeField] private float bananaMult;
    [SerializeField] private float bananaDuration;
    [SerializeField] private float hotdogMult;
    [SerializeField] private float hotdogDuration;
    private Coroutine bananaPowerupCoroutine;
    private Coroutine hotdogPowerupCoroutine;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        moveSpeed = baseSpeed;
        maxSpeed = moveSpeed * maxSpeedMult;

        jumpStrength = baseJumpStrength;
    }

    private void Update()
    {
        maxSpeed = moveSpeed * maxSpeedMult;

        isGrounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.3f, groundLayer);
        if (isGrounded)
            rb.drag = groundDrag;
        else
            rb.drag = 0.0f;

        MoveInput();
    }

    private void FixedUpdate()
    {
        RotateWithCamera();
        MovePlayer();
    }

    private void MoveInput()
    {
        inputX = Input.GetAxisRaw("Horizontal");
        inputY = Input.GetAxisRaw("Vertical");

        if (Input.GetKey(KeyCode.Space) && canJump && isGrounded)
        {
            canJump = false;
            Jump();
            Invoke(nameof(ResetCanJump), jumpCooldown);
        }
    }

    private void MovePlayer()
    {
        moveDirection = rb.transform.forward * inputY + rb.transform.right * inputX;

        if (isGrounded)
        {
            horizontalVelocity = new Vector3(rb.velocity.x, 0.0f, rb.velocity.z);

            if (horizontalVelocity.magnitude < maxSpeed)
                rb.AddForce(moveDirection.normalized * moveSpeed * 10.0f, ForceMode.Force);
        }
        else if (!isGrounded)
        {
            horizontalVelocity = new Vector3(rb.velocity.x, 0.0f, rb.velocity.z);

            if (horizontalVelocity.x > 0.0f)
                horizontalVelocity.x = Mathf.Max(horizontalVelocity.x - airDrag * Time.fixedDeltaTime, 0.0f);
            else if (horizontalVelocity.x < 0.0f)
                horizontalVelocity.x = Mathf.Min(horizontalVelocity.x + airDrag * Time.fixedDeltaTime, 0.0f);

            if (horizontalVelocity.z > 0.0f)
                horizontalVelocity.z = Mathf.Max(horizontalVelocity.z - airDrag * Time.fixedDeltaTime, 0.0f);
            else if (horizontalVelocity.z < 0.0f)
                horizontalVelocity.z = Mathf.Min(horizontalVelocity.z + airDrag * Time.fixedDeltaTime, 0.0f);


            Vector3 airVelocity = horizontalVelocity + (moveDirection.normalized * airControl * Time.fixedDeltaTime);

            if (airVelocity.magnitude > maxSpeed)
                airVelocity = airVelocity.normalized * maxSpeed;

            rb.velocity = new Vector3(airVelocity.x, rb.velocity.y, airVelocity.z);
        }
    }

    private void Jump()
    {
        rb.velocity = new Vector3(rb.velocity.x, 0.0f, rb.velocity.z);
        rb.AddForce(Vector3.up * jumpStrength, ForceMode.Impulse);
    }

    private void ResetCanJump()
    {
        canJump = true;
    }

    private void RotateWithCamera()
    {
        float targetLook = cam.GetYaw();
        Quaternion targetRotation = Quaternion.Euler(0.0f, targetLook, 0.0f);
        rb.rotation = Quaternion.Slerp(rb.rotation, targetRotation, 1000.0f * Time.fixedDeltaTime);
    }
    private IEnumerator HotdogPowerupCoroutine()
    {
        moveSpeed *= hotdogMult;
        maxSpeed = moveSpeed * maxSpeedMult;

        float timeLeft = hotdogDuration;
        while (timeLeft > 0.0f)
        {
            timeLeft -= Time.deltaTime;
            yield return null;
        }

        moveSpeed = baseSpeed;
        maxSpeed = moveSpeed * maxSpeedMult;
        hotdogPowerupCoroutine = null;
    }

    public void HotdogPowerup()
    {
        if (hotdogPowerupCoroutine != null)
        {
            StopCoroutine(hotdogPowerupCoroutine);
            moveSpeed = baseSpeed;
            maxSpeed = moveSpeed * maxSpeedMult;
        }

        hotdogPowerupCoroutine = StartCoroutine(HotdogPowerupCoroutine());
    }

    private IEnumerator BananaPowerupCoroutine()
    {
        jumpStrength *= bananaMult;

        float timeLeft = bananaDuration;
        while (timeLeft > 0.0f)
        {
            timeLeft -= Time.deltaTime;
            yield return null;
        }

        jumpStrength = baseJumpStrength;
        bananaPowerupCoroutine = null;
    }

    public void BananaPowerup()
    {
        if (bananaPowerupCoroutine != null)
        {
            StopCoroutine(bananaPowerupCoroutine);
            moveSpeed = baseSpeed;
            maxSpeed = moveSpeed * maxSpeedMult;
        }

        hotdogPowerupCoroutine = StartCoroutine(BananaPowerupCoroutine());
    }
}
