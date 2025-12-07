using UnityEngine;

public class EvadePlayerState : MovementActionState
{
    private EnemyMovement enemyMovement;
    private float duration = 5.0f;
    private float timer;

    public void Init(EnemyMovement movementReference)
    {
        enemyMovement = movementReference;
    }

    public override void ActivateState()
    {
        timer = 0.0f;
    }

    public override void UpdateState()
    {
        timer += Time.deltaTime;
        enemyMovement.EvadePlayer();
    }

    public override void DeactivateState()
    {
        enemyMovement.StopMoving();
    }

    public override bool FinishState()
    {
        return timer >= duration;
    }
}
