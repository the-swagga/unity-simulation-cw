using UnityEngine;

public class EnemyStats : MonoBehaviour
{
    private PlayerStats playerStats;
    private float playerDamageMult;
    private float playerProjectileSlow;

    private EnemyMovement movement;
    private float originalSpeed;

    [SerializeField] private int baseHP;
    private int hp;
    [SerializeField] private float enemyDamageMult;

    private float bravery = 0.5f;

    private void Start()
    {
        if (playerStats == null)
            playerStats = FindObjectOfType<PlayerStats>();

        if (movement == null)
            movement = GetComponent<EnemyMovement>();

        playerDamageMult = playerStats.GetPlayerDamageMult();
        playerProjectileSlow = playerStats.GetPlayerProjectileSlow();

        originalSpeed = movement.GetSpeed();
        hp = baseHP;
        bravery = Random.Range(0.01f, 1.0f);
        Debug.Log("Bravery " + bravery);
    }

    public float GetBravery()
    {
        return bravery;
    }

    public void ChangeBravery(float change)
    {
        bravery += change;
        if (bravery < 0.01f)
            bravery = 0.01f;
        else if (bravery > 1.0f)
            bravery = 1.0f;
    }

    public void TakeDamage(int damage)
    {
        hp -= damage;
    }

    public int GetHP()
    {
        return hp;
    }

    public float GetEnemyDamageMult()
    {
        return enemyDamageMult;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("PlayerProjectile"))
        {
            float force = collision.impulse.magnitude;
            int damage = Mathf.CeilToInt(force * playerDamageMult);
            TakeDamage(damage);
            ChangeBravery(-0.1f);

            if (movement != null)
                movement.SetSpeed(movement.GetSpeed() * playerProjectileSlow);
        }
    }
    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("PlayerProjectile"))
        {
            float force = collision.impulse.magnitude;
            int damage = Mathf.CeilToInt(force * playerDamageMult * Time.fixedDeltaTime);
            TakeDamage(damage);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("PlayerProjectile"))
        {
            if (movement != null)
                movement.SetSpeed(originalSpeed);
        }
    }
}
