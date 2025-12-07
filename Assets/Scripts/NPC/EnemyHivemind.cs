using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyHivemind : MonoBehaviour
{
    private GameObject player;

    [Header("State Variables")]
    [SerializeField] private float stateDuration;
    [SerializeField] private float aggressionToCollide;
    private bool switchState = false;

    private List<BaseEnemyState> pursueStates = new List<BaseEnemyState>();
    private List<BaseEnemyState> evadeStates = new List<BaseEnemyState>();
    private List<BaseEnemyState> attackStates = new List<BaseEnemyState>();
    private List<BaseEnemyState> collideStates = new List<BaseEnemyState>();
    private List<BaseEnemyState> activeStates = new List<BaseEnemyState>();
    private int enemyCount;
    private List<EnemyStats> enemyStatsList = new List<EnemyStats>();
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

        EnemyDecisions();
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
                    newState = ActAggressive(i, aggression);
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

    private BaseEnemyState ActAggressive(int enemyIndex, float aggression)
    {
        if (aggression >= aggressionToCollide)
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
}
