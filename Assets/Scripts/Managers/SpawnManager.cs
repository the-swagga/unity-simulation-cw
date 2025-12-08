using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private BaseTile tile;

    [SerializeField] private Transform[] hivemindParents;
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private int[] enemyCountRange = new int[2];
    [SerializeField] private float enemySocialDistancing;

    private List<GameObject> enemies = new List<GameObject>();
    private bool enemiesSpawned = false;

    void Update()
    {
        if (tile == null)
            tile = FindObjectOfType<BaseTile>();

        if (!enemiesSpawned && hivemindParents.Length > 0)
        {
            for (int i = 0; i < hivemindParents.Length; i++)
            {
                SpawnEnemies(hivemindParents[i]);
            }
            enemiesSpawned = true;
        }
    }

    private void SpawnEnemies(Transform hivemindParent)
    {
        Vector2 tileSize = tile.GetTileSize();
        Vector3 tilePos = tile.transform.position;

        int enemyCount = Random.Range(enemyCountRange[0], enemyCountRange[1]);
        Vector3 enemyHivemindPos = hivemindParent.position;

        for (int i = 0; i < enemyCount; i++)
        {
            float socialDistancingX = enemySocialDistancing;
            float socialDistancingZ = enemySocialDistancing;

            int flipSignX = Random.Range(0, 2);
            int flipSignZ = Random.Range(0, 2);
            if (flipSignX == 1) socialDistancingX = -socialDistancingX;
            if (flipSignZ == 1) socialDistancingZ = -socialDistancingZ;

            socialDistancingX = Random.Range(socialDistancingX / 2.0f, socialDistancingX);
            socialDistancingZ = Random.Range(socialDistancingZ / 2.0f, socialDistancingZ);
            Vector3 spawnPos = enemyHivemindPos + new Vector3(socialDistancingX, 1.25f, socialDistancingZ);

            GameObject newEnemy = Instantiate(enemyPrefab, spawnPos, Quaternion.identity, hivemindParent);
            enemies.Add(newEnemy);
        }

        enemiesSpawned = true;

        foreach (GameObject enemy in enemies)
        {
            EnemyMovement movement = enemy.GetComponent<EnemyMovement>();
            if (movement != null)
            {
                movement.SetPlayer(player.transform);
            }
        }
    }
}
