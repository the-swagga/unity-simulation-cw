using UnityEngine;

public class BaseEnemyState : MonoBehaviour
{
    public virtual void ActivateState()
    {

    }
    public virtual void UpdateState()
    {
        
    }
    public virtual void DeactivateState()
    {

    }

    public virtual bool FinishState()
    {
        return false;
    }
}
