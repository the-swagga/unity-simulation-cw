using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    private EnemyStats enemyStats;
    private float enemyDamageMult;

    [SerializeField] private GameManager gameManager;
    [SerializeField] private float playerDamageMult;
    [SerializeField] private float playerProjectileSlow;
    [SerializeField] private int baseHP;
    private int hp;

    private void Start()
    {
        hp = baseHP;

        StartCoroutine(DelayedEnemyStats());
    }

    private void Update()
    {
        if (hp <= 0)
            gameManager.Restart();
    }

    private IEnumerator DelayedEnemyStats()
    {
        yield return new WaitForSeconds(1.0f);

        if (enemyStats == null)
            enemyStats = FindFirstObjectByType<EnemyStats>();

        enemyDamageMult = enemyStats.GetEnemyDamageMult();
    }

    public void TakeDamage(int damage)
    {
        hp -= damage;
        Debug.Log("Player HP: " + hp);
    }

    public float GetPlayerDamageMult()
    {
        return playerDamageMult;
    }

    public float GetPlayerProjectileSlow()
    {
        return playerProjectileSlow;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("NPCProjectile"))
        {
            float force = collision.impulse.magnitude;
            int damage = Mathf.CeilToInt(force * enemyDamageMult);
            TakeDamage(damage);

            EnemyProjectile enemyProjectile = collision.gameObject.GetComponent<EnemyProjectile>();
            if (enemyProjectile != null)
            {
                EnemyStats projectileOwner = enemyProjectile.GetOwner();
                if (projectileOwner != null)
                    projectileOwner.ChangeBravery(0.2f);
                
            }
        }

        if (collision.gameObject.layer == LayerMask.NameToLayer("NPC"))
        {
            float force = collision.impulse.magnitude;
            int damage = Mathf.CeilToInt(force * enemyDamageMult);
            TakeDamage(damage);
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("NPCProjectile"))
        {
            float force = collision.impulse.magnitude;
            int damage = Mathf.CeilToInt(force * enemyDamageMult * Time.fixedDeltaTime);
            TakeDamage(damage);
        }

        if (collision.gameObject.layer == LayerMask.NameToLayer("NPC"))
        {
            float force = collision.impulse.magnitude;
            int damage = Mathf.CeilToInt(force * enemyDamageMult * Time.fixedDeltaTime);
            TakeDamage(damage);
        }
    }
}
