using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollidePlayerState : CombatActionState
{
    private EnemyAttack enemyAttack;

    public void Init(EnemyAttack attackReference)
    {
        enemyAttack = attackReference;
    }

    public override void ActivateState()
    {
        
    }
    public override void UpdateState()
    {
        
    }

    public override void DeactivateState()
    {

    }
}
