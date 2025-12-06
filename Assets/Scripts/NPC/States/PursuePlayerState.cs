using UnityEngine;

public class PursuePlayerState : BaseEnemyState
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
}
