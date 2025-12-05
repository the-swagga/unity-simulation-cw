using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody rb;
    [SerializeField] private Transform look;
    private ThirdPersonCamera cam;

    private float inputX;
    private float inputY;
    private Vector3 moveDirection;

    [Header("Movement and Physics Variables")]
    [SerializeField] private float baseSpeed;
    [SerializeField] private float maxSpeedMult;
    [SerializeField] private float groundDrag;
    [SerializeField] private float airDrag;
    [SerializeField] private float baseJumpStrength;
    [SerializeField] private float jumpCooldown;
    [SerializeField] private float airControl;
    [SerializeField] private float baseMass;
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
    [SerializeField] private float hotdogSpeedMult;
    [SerializeField] private float hotDogMassMult;
    [SerializeField] private float hotdogDuration;
    private Coroutine bananaPowerupCoroutine;
    private Coroutine hotdogPowerupCoroutine;

    private void Start()
    {
        cam = FindObjectOfType<ThirdPersonCamera>();

        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        if (baseMass <= 0) baseMass = 1;
        if (hotDogMassMult <= 0) hotDogMassMult = 1;

        rb.mass = baseMass;

        moveSpeed = baseSpeed;
        maxSpeed = moveSpeed * maxSpeedMult;

        jumpStrength = baseJumpStrength;
    }

    private void Update()
    {
        maxSpeed = moveSpeed * maxSpeedMult;

        isGrounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.3f, groundLayer);

        Drag();
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
                rb.AddForce(moveDirection.normalized * moveSpeed * (rb.mass / baseMass) * 10.0f, ForceMode.Force);
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


            Vector3 airVelocity = horizontalVelocity + (moveDirection.normalized * (moveSpeed / hotDogMassMult) * airControl * Time.fixedDeltaTime);

            if (airVelocity.magnitude > maxSpeed)
                airVelocity = airVelocity.normalized * maxSpeed;

            rb.velocity = new Vector3(airVelocity.x, rb.velocity.y, airVelocity.z);
        }
    }

    private void Jump()
    {
        rb.velocity = new Vector3(rb.velocity.x, 0.0f, rb.velocity.z);
        rb.AddForce(Vector3.up * jumpStrength * (rb.mass / baseMass), ForceMode.Impulse);
    }

    private void ResetCanJump()
    {
        canJump = true;
    }

    private void Drag()
    {
        if (isGrounded)
            rb.drag = groundDrag;
        else
            rb.drag = 0.0f;
    }

    private void RotateWithCamera()
    {
        float targetLook = cam.GetYaw();
        Quaternion targetRotation = Quaternion.Euler(0.0f, targetLook, 0.0f);
        rb.rotation = Quaternion.Slerp(rb.rotation, targetRotation, 1000.0f * Time.fixedDeltaTime);
    }
    private IEnumerator HotdogPowerupCoroutine()
    {
        moveSpeed *= hotdogSpeedMult;
        maxSpeed = moveSpeed * maxSpeedMult;
        rb.mass *= hotDogMassMult;

        float timeLeft = hotdogDuration;
        while (timeLeft > 0.0f)
        {
            timeLeft -= Time.deltaTime;
            yield return null;
        }

        moveSpeed = baseSpeed;
        maxSpeed = moveSpeed * maxSpeedMult;
        rb.mass = baseMass;
        hotdogPowerupCoroutine = null;
    }

    public void HotdogPowerup()
    {
        if (hotdogPowerupCoroutine != null)
        {
            StopCoroutine(hotdogPowerupCoroutine);
            moveSpeed = baseSpeed;
            maxSpeed = moveSpeed * maxSpeedMult;
            rb.mass = baseMass;
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

        bananaPowerupCoroutine = StartCoroutine(BananaPowerupCoroutine());
    }

    public void SetMaterial(Material newMaterial)
    {
        MeshRenderer meshRenderer = this.GetComponentInChildren<MeshRenderer>();
        if (meshRenderer != null)
        {
            meshRenderer.material = newMaterial;
        }
    }

    public float GetPhysicsMaterialBounciness()
    {
        Collider col = this.GetComponentInChildren<Collider>();
        if (col != null)
        {
            return col.material.bounciness;
        } else return 0.0f;
    }

    public PhysicMaterialCombine GetPhysicsMaterialBounceCombine()
    {
        Collider col = this.GetComponentInChildren<Collider>();
        if (col != null)
        {
            return col.material.bounceCombine;
        }
        else return PhysicMaterialCombine.Minimum;
    }

    public void SetPhysicsMaterialBounciness(float newBounciness, PhysicMaterialCombine newBounceCombine)
    {
        Collider col = this.GetComponentInChildren<Collider>();
        if (col != null)
        {
            if (col.material == null)
            {
                col.material = new PhysicMaterial();
            }

            col.material.bounciness = newBounciness;
            col.material.bounceCombine = newBounceCombine;
        }
    }
}
