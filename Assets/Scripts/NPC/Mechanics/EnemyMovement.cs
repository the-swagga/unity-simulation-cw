using UnityEngine;
using UnityEngine.AI;

public class EnemyMovement : MonoBehaviour
{
    [SerializeField] private NavMeshAgent agent;
    private Transform player;

    [Header("Movement Variables")]
    [SerializeField] private float[] speedRange = new float[2];

    private void Start()
    {
        if (agent == null)
            agent = GetComponent<NavMeshAgent>();

        if (agent != null && player != null)
        {
            SetRandomSpeed();
        }
    }

    public void SetPlayer(Transform playerTransform)
    {
        player = playerTransform;
    }

    private void SetRandomSpeed()
    {
        float newSpeed = Random.Range(speedRange[0], speedRange[1]);
        agent.speed = newSpeed;
    }

    // State Functions

    public void SetDestToPlayer()
    {
        agent.isStopped = false;
        agent.SetDestination(player.position);
    }

    public void EvadePlayer()
    {
        agent.isStopped = false;
        Vector3 target = new Vector3(0.0f, transform.position.y, 0.0f);

        agent.SetDestination(target);
    }

    public void StopMoving()
    {
        agent.isStopped = true;
    }
}
