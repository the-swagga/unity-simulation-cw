using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    private EnemyStats owner;

    public EnemyStats GetOwner()
    {
        return owner;
    }

    public void SetOwner(EnemyStats newOwner)
    {
        owner = newOwner;
    }
}
