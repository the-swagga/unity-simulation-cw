using System.Collections;
using UnityEngine;

public class PlayerCannon : MonoBehaviour
{
    //[Header("Particle Variables")]
    private ParticleSystem fireParticleSystem;

    [Header("Firing Variables")]
    [SerializeField] private Transform fireOrigin;
    [SerializeField] private GameObject cannonball;
    [SerializeField] private float fireRate;
    [SerializeField] private float fireForce;
    [SerializeField] private int poolSize;

    [Header("Powerup Variables")]
    [SerializeField] private PhysicMaterial cannonballPhysicsMat;
    [SerializeField] private float cherryBounciness;
    [SerializeField] private float cherryDuration;
    [SerializeField] private Material playerMat;
    [SerializeField] private Material cherryMat;
    private Coroutine cherryPowerupCoroutine;
    private float baseBounciness;
    private float basePlayerBounciness;
    private PhysicMaterialCombine basePlayerBounceCombine;

    [Header("TrajectoryVariables")]
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private int linePoints = 30;
    [SerializeField] private Material lineMaterial;

    [Header("Scripts")]
    [SerializeField] private PlayerController playerController;
    [SerializeField] private PlayerCannonController cannonController;
    [SerializeField] private ThirdPersonCamera playerCam;
    private bool isAiming;

    private GameObject[] cannonballPool;
    private int currentIndex = 0;

    public void SetAim(bool aiming, ThirdPersonCamera camera)
    {
        isAiming = aiming;
        playerCam = camera;
    }

    private void Start()
    {
        if (playerController == null)
        {
            playerController = FindObjectOfType<PlayerController>();
        } else
        {
            basePlayerBounciness = playerController.GetPhysicsMaterialBounciness();
            basePlayerBounceCombine = playerController.GetPhysicsMaterialBounceCombine();
        }

        if (cannonController == null)
            cannonController = FindObjectOfType<PlayerCannonController>();

        if (playerCam == null)
            playerCam = FindObjectOfType<ThirdPersonCamera>();

        if (fireParticleSystem == null)
            fireParticleSystem = GetComponentInChildren<ParticleSystem>();

        baseBounciness = cannonballPhysicsMat.bounciness;

        CannonballPoolInit();
    }

    private void Update()
    {
        lineRenderer.material = lineMaterial;

        if (isAiming)
        {
            if (lineRenderer != null && !lineRenderer.enabled) lineRenderer.enabled = true;

            AimAtCrosshair();
            DrawTrajectory();

            if (Input.GetMouseButtonDown(0)) FireCannonball();
        }
        else
        {
            if (lineRenderer != null && lineRenderer.enabled) lineRenderer.enabled = false;
        }
    }

    private void CannonballPoolInit()
    {
        cannonballPool = new GameObject[poolSize];
        for (int i = 0; i < poolSize; i++)
        {
            GameObject newCannonball = Instantiate(cannonball, Vector3.zero, Quaternion.identity);
            Collider col = newCannonball.GetComponent<Collider>();
            if (col != null)
            {
                PhysicMaterial physicsMatInstance = new PhysicMaterial();
                physicsMatInstance.bounciness = baseBounciness;
                physicsMatInstance.dynamicFriction = cannonballPhysicsMat.dynamicFriction;
                physicsMatInstance.staticFriction = cannonballPhysicsMat.staticFriction;
                physicsMatInstance.frictionCombine = cannonballPhysicsMat.frictionCombine;
                physicsMatInstance.bounceCombine = cannonballPhysicsMat.bounceCombine;

                col.material = physicsMatInstance;
            }

            newCannonball.SetActive(false);
            cannonballPool[i] = newCannonball;
        }
    }

    private void AimAtCrosshair()
    {
        if (playerCam == null) return;

        float pitch = playerCam.GetPitch();
        float yaw = 0.0f;
        if (transform.parent != null) yaw = transform.parent.eulerAngles.y;

        transform.rotation = Quaternion.Euler(pitch + 90.0f, yaw, 0.0f);
    }

    private void DrawTrajectory()
    {
        if (lineRenderer == null || fireOrigin == null || cannonball == null) return;

        Rigidbody cannonballRb = cannonball.GetComponent<Rigidbody>();
        if (cannonballRb == null || cannonballRb.mass <= 0.0f) return;

        Vector3 startPos = fireOrigin.position;
        Vector3 u = fireOrigin.up * (fireForce / cannonballRb.mass); // initial velocity = impulse / mass

        float y0 = startPos.y; // initial vertical distance
        float uy = u.y; // initial vertical velocity
        float ay = Physics.gravity.y; // vertical acceleration
        // s = s0 + ut + 0.5a*t^2 -> 0 = y0 + uy*t + 0.5*ay*t^2 -> ay*t^2 + 2uy*t + 2y0 = 0
        float a = ay;
        float b = 2 * uy;
        float c = 2 * y0;
        float discriminant = (b * b) - (4 * a * c); // calculate discriminant to avoid issues
        if (discriminant < 0.0f) return;
        float sqrtDiscriminant = Mathf.Sqrt(discriminant);
        float t1 = (-b + sqrtDiscriminant) / (2 * a);
        float t2 = (-b - sqrtDiscriminant) / (2 * a);
        float t = Mathf.Max(t1, t2); // take positive time solution

        Vector3[] points = new Vector3[linePoints];
        for (int i = 0; i < linePoints; i++)
        {
            float timeSlice = (t / (linePoints - 1)) * i;
            Vector3 s = (u * timeSlice) + 0.5f * (Physics.gravity * (timeSlice * timeSlice));
            points[i] = startPos + s;
        }

        lineRenderer.positionCount = linePoints;
        lineRenderer.SetPositions(points);
    }

    private void FireCannonball()
    {
        GameObject nextCannonball = cannonballPool[currentIndex];
        Rigidbody rb = nextCannonball.GetComponent<Rigidbody>();

        if (rb == null) return;

        nextCannonball.SetActive(true);
        nextCannonball.transform.position = fireOrigin.position;
        nextCannonball.transform.rotation = fireOrigin.rotation;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        rb.AddForce(fireOrigin.up * fireForce, ForceMode.Impulse);

        currentIndex++;
        if (currentIndex == poolSize)
            currentIndex = 0;

        if (fireParticleSystem != null)
            fireParticleSystem.Play();
    }

    private IEnumerator CherryPowerupCoroutine()
    {
        foreach (var cannonball in cannonballPool)
        {
            Collider col = cannonball.GetComponent<Collider>();
            if (col != null && col.material != null) col.material.bounciness = cherryBounciness;
            cannonController.SetCanSwap(false);
        }

        playerController.SetMaterial(cherryMat);
        playerController.SetPhysicsMaterialBounciness(1.0f, PhysicMaterialCombine.Maximum);

        float timeLeft = cherryDuration;
        while (timeLeft > 0.0f)
        {
            timeLeft -= Time.deltaTime;
            yield return null;
        }

        foreach (var cannonball in cannonballPool)
        {
            Collider col = cannonball.GetComponent<Collider>();
            if (col != null && col.material != null) col.material.bounciness = baseBounciness;
            cannonController.SetCanSwap(true);
        }

        playerController.SetMaterial(playerMat);
        playerController.SetPhysicsMaterialBounciness(basePlayerBounciness, basePlayerBounceCombine);

        cherryPowerupCoroutine = null;
    }

    public void CherryPowerup()
    {
        if (cherryPowerupCoroutine != null)
        {
            StopCoroutine(cherryPowerupCoroutine);

            foreach (var cannonball in cannonballPool)
            {
                Collider col = cannonball.GetComponent<Collider>();
                if (col != null && col.material != null) col.material.bounciness = baseBounciness;
                cannonController.SetCanSwap(true);
            }

            playerController.SetPhysicsMaterialBounciness(basePlayerBounciness, basePlayerBounceCombine);
        }

        cherryPowerupCoroutine = StartCoroutine(CherryPowerupCoroutine());
    }
}
