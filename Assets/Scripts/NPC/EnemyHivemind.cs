using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyHivemind : MonoBehaviour
{
    private List<BaseEnemyState> pursueStates = new List<BaseEnemyState>();
    private List<BaseEnemyState> evadeStates = new List<BaseEnemyState>();
    private List<BaseEnemyState> activeStates = new List<BaseEnemyState>();

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

        Test();
    }

    private void InitEnemyStates()
    {
        foreach (Transform enemy in transform)
        {
            EnemyMovement movement = enemy.GetComponent<EnemyMovement>();
            PursuePlayerState pursuePlayer = enemy.GetComponent<PursuePlayerState>();
            EvadePlayerState evadePlayer = enemy.GetComponent<EvadePlayerState>();

            if (movement != null)
            {
                if (pursuePlayer != null)
                    pursuePlayer.Init(movement);
                if (evadePlayer != null)
                    evadePlayer.Init(movement);
            }

            if (pursuePlayer != null && evadePlayer != null)
            {
                pursueStates.Add(pursuePlayer);
                evadeStates.Add(evadePlayer);

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
    }

    private void Test()
    {
        switchTimer += Time.deltaTime;

        if (switchTimer >= stateSwitch)
        {
            switchTimer = 0.0f;

            for (int i = 0; i < activeStates.Count; i++)
            {
                BaseEnemyState oldState = activeStates[i];
                BaseEnemyState newState;

                if (oldState is PursuePlayerState)
                {
                    newState = evadeStates[i];
                }
                else if (oldState is EvadePlayerState)
                {
                    newState = pursueStates[i];
                }
                else
                {
                    newState = oldState;
                }

                oldState.DeactivateState();
                newState.ActivateState();
                activeStates[i] = newState;

                Debug.Log($"Enemy {activeStates[i].name} switched from {oldState.GetType().Name} to {newState.GetType().Name}");
            }
        }
    }
}
