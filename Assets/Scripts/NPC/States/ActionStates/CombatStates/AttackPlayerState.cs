using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackPlayerState : CombatActionState
{
    private EnemyAttack enemyAttack;
    private float duration = 5.0f;
    private float timer;

    public void Init(EnemyAttack attackReference)
    {
        enemyAttack = attackReference;
    }

    public override void ActivateState()
    {
        timer = 0.0f;
        enemyAttack.ResetFireRateTimer();
    }
    public override void UpdateState()
    {
        timer += Time.deltaTime;

        if (enemyAttack.CanHitTarget(enemyAttack.GetPlayerPos()))
            enemyAttack.TryFire();
    }

    public override void DeactivateState()
    {
        
    }

    public override bool FinishState()
    {
        return timer >= duration;
    }
}
