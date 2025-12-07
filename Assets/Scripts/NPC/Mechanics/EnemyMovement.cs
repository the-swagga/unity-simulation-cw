using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class EnemyMovement : MonoBehaviour
{
    [SerializeField] private NavMeshAgent agent;
    private Transform player;

    [Header("Movement Variables")]
    [SerializeField] private float[] speedRange = new float[2];
    [SerializeField] private float[] getCloserRange = new float[2];

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

    public void GetCloser(Vector3 target)
    {
        StartCoroutine(GetCloserCoroutine(target));
    }

    private IEnumerator GetCloserCoroutine(Vector3 target)
    {
        float timer = Random.Range(getCloserRange[0], getCloserRange[1]);
        float closerDistance = timer * 25.0f;

        Vector3 direction = (target - transform.position).normalized;
        Vector3 moveTo = transform.position + (direction * closerDistance);
        agent.isStopped = false;
        agent.SetDestination(moveTo);

        while (timer < 0.25f)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        agent.isStopped = true;
    }

    public void EvadePlayer()
    {
        agent.isStopped = false;

        Vector3 awayFromPlayer = (transform.position - player.position).normalized;
        Vector3 target = transform.position + awayFromPlayer * 50.0f;
        target.y = transform.position.y;
        agent.SetDestination(target);
    }

    public void StopMoving()
    {
        agent.isStopped = true;
    }
}
