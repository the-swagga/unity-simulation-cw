using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyHivemind : MonoBehaviour
{
    [Header("State Variables")]
    [SerializeField] private float aggressionToCollide;
    [SerializeField] private float evasionLockoutDuration;
    private bool evasionLockout = false;

    private List<BaseEnemyState> pursueStates = new List<BaseEnemyState>();
    private List<BaseEnemyState> evadeStates = new List<BaseEnemyState>();
    private List<BaseEnemyState> attackStates = new List<BaseEnemyState>();
    private List<BaseEnemyState> collideStates = new List<BaseEnemyState>();
    private List<BaseEnemyState> activeStates = new List<BaseEnemyState>();
    private int enemyCount;
    private List<EnemyStats> enemyStatsList = new List<EnemyStats>();
    private int currentEnemyHP;
    private float currentEnemyBravery;
    //private List<EnemyAttack> enemyAttackList = new List<EnemyAttack>();

    private float stateSwitch = 5.0f;
    private float switchTimer = 0.0f;

    private bool initialised = false;

    private void Start()
    {
        StartCoroutine(DelayedInit());
    }

    private void Update()
    {
        if (!initialised) return;

        foreach (var state in activeStates)
            state.UpdateState();

        EnemyDecisions();
    }

    private void InitEnemyStates()
    {
        foreach (Transform enemy in transform)
        {
            EnemyMovement movement = enemy.GetComponent<EnemyMovement>();
            EnemyStats stats = enemy.GetComponent<EnemyStats>();
            // EnemyAttack attack = enemy.GetComponent<EnemyAttack>();
            PursuePlayerState pursuePlayer = enemy.GetComponent<PursuePlayerState>();
            EvadePlayerState evadePlayer = enemy.GetComponent<EvadePlayerState>();
            AttackPlayerState attackPlayer = enemy.GetComponent<AttackPlayerState>();
            CollidePlayerState collidePlayer = enemy.GetComponent<CollidePlayerState>();

            enemyStatsList.Add(stats);
            // enemyAttackList.Add(attack);

            if (movement != null)
            {
                if (pursuePlayer != null)
                    pursuePlayer.Init(movement);
                if (evadePlayer != null)
                    evadePlayer.Init(movement);
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
        if (evasionLockout) return;

        switchTimer += Time.deltaTime;

        if (switchTimer >= stateSwitch)
        {
            switchTimer = 0.0f;

            for (int i = 0; i < enemyCount; i++)
            {
                BaseEnemyState oldState = activeStates[i];
                BaseEnemyState newState;
                currentEnemyHP = enemyStatsList[i].GetHP();
                currentEnemyBravery = enemyStatsList[i].GetBravery();

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

                if (newState != oldState)
                {
                    oldState.DeactivateState();
                    newState.ActivateState();
                    activeStates[i] = newState;
                }
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
        // code to get range from EnemyAttack, compare distance to player with range
        float placeholderDistance = 5.0f;

        if (aggression >= aggressionToCollide)
            return collideStates[enemyIndex];

        //if (placeholderDistance <= enemyAttackList[enemyIndex].GetRange())
            //return attackStates[enemyIndex];

        return pursueStates[enemyIndex];
    }

    private BaseEnemyState ActPassive(int enemyIndex)
    {
        if (!evasionLockout)
            StartCoroutine(EvasionLockoutCoroutine());

        return evadeStates[enemyIndex];
    }

    private IEnumerator EvasionLockoutCoroutine()
    {
        evasionLockout = true;
        yield return new WaitForSeconds(evasionLockoutDuration);
        evasionLockout = false;
    }
}
