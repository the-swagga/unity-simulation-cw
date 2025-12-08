using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    private GameObject player;
    private EnemyMovement movement;

    [Header("Firing Variables")]
    [SerializeField] private Transform fireOrigin;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float fireRate;
    [SerializeField] private float maxFireForce;
    [SerializeField] private int poolSize;

    private GameObject[] projectilePool;
    private int currentIndex = 0;

    private float fireRateTimer = 0.0f;

    private void Start()
    {
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player");

        if (movement == null)
            movement = GetComponent<EnemyMovement>();

        ProjectilePoolInit();
    }

    private void ProjectilePoolInit()
    {
        projectilePool = new GameObject[poolSize];
        for (int i = 0; i < poolSize; i++)
        {
            GameObject newProjectile = Instantiate(projectilePrefab, Vector3.zero, Quaternion.identity);
            newProjectile.SetActive(false);
            projectilePool[i] = newProjectile;
        }
    }

    public void CollideWithPlayer()
    {
        movement.CollidePlayerMovement(player.transform);
    }

    public bool CanHitTarget(Vector3 targetPos)
    {
        float requiredInitialVelocity = 0.0f;

        Vector3 distance = targetPos - fireOrigin.position;

        float verticalDistance = distance.y;
        Vector3 horizontalVector = new Vector3(distance.x, 0.0f, distance.z);
        float horizontalDistance = horizontalVector.magnitude;

        float ay = Physics.gravity.y;
        float th = 45.0f * Mathf.Deg2Rad;
        float costh = Mathf.Cos(th);
        float tanth = Mathf.Tan(th);

        // Horizontal: sx = u * costh * t -> t = sx / (u * costh)
        // Vertical: sy = (u * sinth * t) + (0.5 * ay * t^2)
        // Substitute t from horizontal into vertical -> sy = (u * sinth * (sx / (u * costh))) + (0.5 * ay * (sx / (u * costh))^2)
        // Rearrange to isolate u

        float numerator = 0.5f * ay * horizontalDistance * horizontalDistance;
        float denominator = verticalDistance - ((horizontalDistance * tanth) * (costh * costh));
        if (denominator == 0.0f)
        {
            requiredInitialVelocity = 0.0f;
            return false;
        }
        float u = Mathf.Sqrt(numerator / denominator);

        if (u <= maxFireForce)
        {
            requiredInitialVelocity = u;
            return true;
        }
        else
        {
            requiredInitialVelocity = 0.0f;
            return false;
        }
    }

    private void FireProjectile()
    {
        if (player == null) return;

        Vector3 distance = player.transform.position - fireOrigin.position;
        Vector3 horizontalVector = new Vector3(distance.x, 0.0f, distance.z);
        Vector3 horizontalDirection = horizontalVector.normalized;

        float th = FireAngle(player.transform.position);
        if (th == 0.0f) return;
        float costh = Mathf.Cos(th);
        float sinth = Mathf.Sin(th);

        Vector3 launchForce = (horizontalDirection * (maxFireForce * costh)) + (Vector3.up * (maxFireForce * sinth));

        GameObject nextProjectile = projectilePool[currentIndex];
        Rigidbody rb = nextProjectile.GetComponent<Rigidbody>();

        if (rb == null) return;

        nextProjectile.SetActive(true);
        nextProjectile.transform.position = fireOrigin.position;
        nextProjectile.transform.rotation = fireOrigin.rotation;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        rb.AddForce(launchForce * rb.mass, ForceMode.Impulse);

        currentIndex++;
        if (currentIndex >= poolSize)
            currentIndex = 0;
    }

    private float FireAngle(Vector3 target)
    {
        Vector3 distance = target - fireOrigin.position;
        float sy = distance.y;
        Vector3 horizontalVector = new Vector3(distance.x, 0.0f, distance.z);
        float sx = horizontalVector.magnitude;
        float u = maxFireForce;
        float ay = Mathf.Abs(Physics.gravity.y);

        float discriminant = (u * u * u * u) - ay * (ay * sx * sx + 2f * sy * u * u);
        if (discriminant < 0.0f)
        {
            return 0.0f;
        }
        float sqrtDiscriminant = Mathf.Sqrt(discriminant);
        float tanth1 = ((u * u) + sqrtDiscriminant) / (ay * sx);
        float tanth2 = ((u * u) - sqrtDiscriminant) / (ay * sx);
        float angle = Mathf.Atan(Mathf.Min(tanth1, tanth2));
        return angle;
    }

    public void TryFire()
    {
        if (player == null) return;

        fireRateTimer += Time.deltaTime;
        if (fireRateTimer >= fireRate)
        {
            FaceTarget(player.transform.position);
            fireRateTimer = 0.0f;
            if (CanHitTarget(player.transform.position))
            {
                FireProjectile();
            }
        }
    }

    private void FaceTarget(Vector3 target)
    {
        Vector3 direction = target - transform.position;
        direction.y = 0.0f;

        Quaternion look = Quaternion.LookRotation(direction);
        transform.rotation = look;
    }

    public void ResetFireRateTimer()
    {
        fireRateTimer = fireRate;
    }

    public Vector3 GetPlayerPos()
    {
        return player.transform.position;
    }
}
