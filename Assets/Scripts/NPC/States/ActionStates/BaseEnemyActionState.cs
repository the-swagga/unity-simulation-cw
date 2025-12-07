using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseEnemyActionState : BaseEnemyState
{
    protected BaseEnemyState currentSubState;

    public void SetCurrentSubState(BaseEnemyState subState)
    {
        if (currentSubState != null)
            currentSubState.DeactivateState();

        currentSubState = subState;

        if (currentSubState != null)
            currentSubState.ActivateState();
    }

    public override void UpdateState()
    {
        if (currentSubState != null)
            currentSubState.UpdateState();
    }

    public override void DeactivateState()
    {
        if (currentSubState != null)
            currentSubState.DeactivateState();
    }

    public override bool FinishState()
    {
        if (currentSubState != null)
            currentSubState.FinishState();

        return false;
    }
}
