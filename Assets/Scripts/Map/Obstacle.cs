using UnityEngine;
using UnityEngine.AI;

public class Obstacle : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float moveDistance;

    private NavMeshObstacle obstacle;
    private Vector3 startPos;

    private void Start()
    {
        obstacle = GetComponent<NavMeshObstacle>();
        if (obstacle != null)
        {
            obstacle.carving = true;
            obstacle.carveOnlyStationary = false;
        }

        startPos = transform.position;
    }

    private void Update()
    {
        Vector3 direction = Vector3.right.normalized;
        Vector3 offset = direction * Mathf.Sin(Time.time * moveSpeed) * moveDistance;
        transform.position = startPos + offset;
    }
}
