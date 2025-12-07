using UnityEngine;

public class PursuePlayerState : MovementActionState
{
    private EnemyMovement enemyMovement;

    public void Init(EnemyMovement movementReference)
    {
        enemyMovement = movementReference;
    }

    public override void ActivateState()
    {
        
    }
    public override void UpdateState()
    {
        enemyMovement.SetDestToPlayer();
    }

    public override void DeactivateState()
    {
        enemyMovement.StopMoving();
    }

    public override bool FinishState()
    {
        EnemyAttack enemyAttack = GetComponent<EnemyAttack>();
        if (enemyAttack != null && enemyAttack.CanHitTarget(enemyAttack.GetPlayerPos()))
        {
            enemyMovement.GetCloser(enemyAttack.GetPlayerPos());
            return true;
        } else { return false; }
    }
}
