using UnityEngine;

public class EvadePlayerState : MovementActionState
{
    private EnemyMovement enemyMovement;

    public void Init(EnemyMovement movementReference)
    {
        enemyMovement = movementReference;
    }

    public override void ActivateState()
    {
        enemyMovement.EvadePlayer();
    }

    public override void UpdateState()
    {
        
    }

    public override void DeactivateState()
    {
        enemyMovement.StopMoving();
    }
}
