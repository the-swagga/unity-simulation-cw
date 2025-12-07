using UnityEngine;

public class EnemyStats : MonoBehaviour
{
    [SerializeField] private int hp;

    private float bravery = 0.5f;

    private void Update()
    {
        CheckHP();
    }

    public float GetBravery()
    {
        return bravery;
    }

    public void ChangeBravery(float change)
    {
        bravery += change;
        if (bravery < 0.0f)
            bravery = 0.0f;
        else if (bravery > 1.0f)
            bravery = 1.0f;
    }

    public void TakeDamage(int damage)
    {
        hp -= damage;
        bravery -= 0.1f;
    }

    public int GetHP()
    {
        return hp;
    }

    private void CheckHP()
    {
        if (hp <= 0)
            Destroy(gameObject);
    }
}
