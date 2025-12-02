using UnityEngine;

public class PlayerCannon : MonoBehaviour
{
    [Header("Firing Variables")]
    [SerializeField] private Transform fireOrigin;
    [SerializeField] private GameObject cannonball;
    [SerializeField] private float fireRate = 1.0f;
    [SerializeField] private float fireForce = 250.0f;
    [SerializeField] private int poolSize = 5;

    [Header("TrajectoryVariables")]
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private int linePoints = 30;
    [SerializeField] private Material lineMaterial;

    private bool isAiming;
    [SerializeField] private ThirdPersonCamera playerCam;

    private GameObject[] cannonballPool;
    private int currentIndex = 0;

    public void SetAim(bool aiming, ThirdPersonCamera camera)
    {
        isAiming = aiming;
        playerCam = camera;
    }

    private void Start()
    {
        cannonballPool = new GameObject[poolSize];
        for (int i = 0; i < poolSize; i++)
        {
            GameObject newCannonball = Instantiate(cannonball, Vector3.zero, Quaternion.identity);
            newCannonball.SetActive(false);
            cannonballPool[i] = newCannonball;
        }
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
        if (currentIndex == poolSize) currentIndex = 0;
    }
}
