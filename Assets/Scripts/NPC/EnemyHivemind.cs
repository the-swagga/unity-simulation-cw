using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHivemind : MonoBehaviour
{
    private GameObject player;

    [Header("State Variables")]
    [SerializeField] private float stateDuration;
    [SerializeField] private float aggressionToCollide;

    private List<BaseEnemyState> pursueStates = new List<BaseEnemyState>();
    private List<BaseEnemyState> evadeStates = new List<BaseEnemyState>();
    private List<BaseEnemyState> attackStates = new List<BaseEnemyState>();
    private List<BaseEnemyState> collideStates = new List<BaseEnemyState>();
    private List<BaseEnemyState> activeStates = new List<BaseEnemyState>();
    private int enemyCount;
    private List<EnemyStats> enemyStatsList = new List<EnemyStats>();
    private int enemyBaseHP;
    private int currentEnemyHP;
    private float currentEnemyBravery;
    private List<EnemyAttack> enemyAttackList = new List<EnemyAttack>();

    private bool initialised = false;

    private void Start()
    {
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player");

        StartCoroutine(DelayedInit());
    }

    private void Update()
    {
        if (!initialised) return;

        CheckEnemyDead();
        EnemyDecisions();
    }

    private void CheckEnemyDead()
    {
        for (int i = activeStates.Count - 1;  i >= 0; i--)
        {
            EnemyStats stats = enemyStatsList[i];
            if (stats != null && stats.GetHP() <= 0)
            {
                Destroy(stats.gameObject);
                enemyStatsList.RemoveAt(i);
                enemyAttackList.RemoveAt(i);
                pursueStates.RemoveAt(i);
                evadeStates.RemoveAt(i);
                attackStates.RemoveAt(i);
                collideStates.RemoveAt(i);
                activeStates.RemoveAt(i);
            }
        }

        enemyCount = activeStates.Count;
    }

    private void InitEnemyStates()
    {
        foreach (Transform enemy in transform)
        {
            EnemyMovement movement = enemy.GetComponent<EnemyMovement>();
            EnemyStats stats = enemy.GetComponent<EnemyStats>();
            EnemyAttack attack = enemy.GetComponent<EnemyAttack>();
            PursuePlayerState pursuePlayer = enemy.GetComponent<PursuePlayerState>();
            EvadePlayerState evadePlayer = enemy.GetComponent<EvadePlayerState>();
            AttackPlayerState attackPlayer = enemy.GetComponent<AttackPlayerState>();
            CollidePlayerState collidePlayer = enemy.GetComponent<CollidePlayerState>();

            enemyBaseHP = stats.GetHP();

            enemyStatsList.Add(stats);
            enemyAttackList.Add(attack);

            if (movement != null)
            {
                if (pursuePlayer != null)
                    pursuePlayer.Init(movement);
                if (evadePlayer != null)
                    evadePlayer.Init(movement);
            }

            if (attack != null)
            {
                if (attackPlayer != null)
                    attackPlayer.Init(attack);
                if (collidePlayer != null)
                    collidePlayer.Init(attack);
            }

            if (pursuePlayer != null && evadePlayer != null && attackPlayer != null && collidePlayer != null)
            {
                pursueStates.Add(pursuePlayer);
                evadeStates.Add(evadePlayer);
                attackStates.Add(attackPlayer);
                collideStates.Add(collidePlayer);

                activeStates.Add(pursuePlayer);
                pursuePlayer.ActivateState();
            }
        }
    }

    private IEnumerator DelayedInit()
    {
        yield return new WaitForSeconds(1.0f);
        InitEnemyStates();
        initialised = true;
        enemyCount = activeStates.Count;
    }

    private void EnemyDecisions()
    {
        if (enemyCount <= 0) return;

        for (int i = 0; i < enemyCount; i++)
        {
            BaseEnemyState currentState = activeStates[i];
            currentState.UpdateState();

            currentEnemyHP = enemyStatsList[i].GetHP();
            currentEnemyBravery = enemyStatsList[i].GetBravery();

            BaseEnemyState newState = currentState;
            if (currentState.FinishState())
            {
                // Odds to pursue and attack: ((Hivemind Health * Hivemind Bravery) - Individual Health) * Individual Bravery
                int hivemindHealth = HivemindHP();
                float hivemindBravery = HivemindAvgBravery();
                float aggression = ((hivemindHealth * hivemindBravery) - currentEnemyHP) * currentEnemyBravery;
                aggression = Mathf.InverseLerp(-50.0f, 50.0f, aggression);

                float randomDecision = Random.Range(0.0f, 1.0f);
                if (randomDecision < aggression)
                    newState = ActAggressive(i, aggression, currentEnemyHP);
                else
                    newState = ActPassive(i);
            }

            if (newState != currentState)
            {
                currentState.DeactivateState();
                newState.ActivateState();
                activeStates[i] = newState;
            }
        }
    }

    private int HivemindHP()
    {
        int health = 0;

        for (int i = 0; i < enemyCount; i++)
        {
            health += enemyStatsList[i].GetHP();
        }

        return health;
    }

    private float HivemindAvgBravery()
    {
        float bravery = 0;

        for (int i = 0; i < enemyCount; i++)
        {
            bravery += enemyStatsList[i].GetBravery();
        }

        bravery = bravery / enemyCount;

        return bravery;
    }

    private BaseEnemyState ActAggressive(int enemyIndex, float aggression, float hp)
    {
        if (aggression >= aggressionToCollide && hp <= (enemyBaseHP * 0.25f))
        {
            Debug.Log("Colliding");
            return collideStates[enemyIndex];
        }
            

        EnemyAttack attack = enemyAttackList[enemyIndex];
        if (attack.CanHitTarget(player.transform.position))
        {
            Debug.Log("Attacking");
            return attackStates[enemyIndex];
        }

        Debug.Log("Pursuing");
        return pursueStates[enemyIndex];
    }

    private BaseEnemyState ActPassive(int enemyIndex)
    {
        Debug.Log("Evading");
        return evadeStates[enemyIndex];
    }

    public Vector3 HivemindCentre()
    {
        List<Vector3> enemyPositions = new List<Vector3>();
        foreach (Transform enemy in transform)
        {
            enemyPositions.Add(enemy.position);
        }

        Vector3 centre = Vector3.zero;
        foreach (Vector3 position in enemyPositions)
        {
            centre += position;
        }

        centre /= enemyPositions.Count;

        return centre;
    }
}
