using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class EnemyMovement : MonoBehaviour
{
    private EnemyHivemind enemyHivemind;
    [SerializeField] private NavMeshAgent agent;
    private Transform player;

    [Header("Movement Variables")]
    [SerializeField] private float[] speedRange = new float[2];
    [SerializeField] private float[] getCloserRange = new float[2];
    [SerializeField] private float[] evadeRange = new float[2];
    [SerializeField] private float[] collideSpeedMult = new float[2];
    private float baseSpeed;

    private void Start()
    {
        if (enemyHivemind == null)
            enemyHivemind = GetComponentInParent<EnemyHivemind>();

        if (agent == null)
            agent = GetComponent<NavMeshAgent>();

        SetRandomSpeed();

        baseSpeed = agent.speed;
    }

    public void SetPlayer(Transform playerTransform)
    {
        player = playerTransform;
    }

    public float GetSpeed()
    {
        return agent.speed;
    }

    public void SetSpeed(float speed)
    {
        agent.speed = speed;
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

        Vector3 hivemindCentre = enemyHivemind.HivemindCentre();
        Vector3 playerRelativeDirection = (hivemindCentre - player.position).normalized;
        playerRelativeDirection.y = 0.0f;

        float evadeDistance = Random.Range(evadeRange[0], evadeRange[1]);
        Vector3 target = hivemindCentre + (playerRelativeDirection * evadeDistance);
        target.y = transform.position.y;

        agent.SetDestination(target);
    }

    public void StopMoving()
    {
        agent.isStopped = true;
    }

    public void CollidePlayerMovement(Transform target)
    {
        agent.isStopped = false;
        agent.SetDestination(target.position);

        float speedMult = Random.Range(collideSpeedMult[0], collideSpeedMult[1]);
        agent.speed = baseSpeed * speedMult;
    }
}
