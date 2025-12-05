using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private TileManager tileManager;

    private Vector2Int currentTile;

    [SerializeField] private Transform entitiesParent;
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private int[] enemyCountRange = new int[2];
    [SerializeField] private float enemySocialDistancing;

    private List<GameObject> enemies = new List<GameObject>();
    private bool enemiesSpawned = false;
    private Vector2Int spawnedTile;

    void Start()
    {

    }

    void Update()
    {
        if (tileManager == null) return;

        currentTile = tileManager.GetCurrentTile();

        if (!enemiesSpawned)
            SpawnEnemies();
    }

    private void SpawnEnemies()
    {
        if (tileManager == null) return;

        if (tileManager.GetPlayerTileTag() != "RocklandsTile") return;

        var tile = tileManager.GetActiveTiles()[currentTile];
        Vector2 tileSize = tile.GetTileSize();
        Vector3 tilePos = tile.transform.position;

        int enemyCount = Random.Range(enemyCountRange[0], enemyCountRange[1]);
        float enemyGroupX = Random.Range(-tileSize.x / 2.0f, tileSize.x / 2.0f);
        float enemyGroupZ = Random.Range(-tileSize.y / 2.0f, tileSize.y / 2.0f);
        Vector3 enemyGroupPos = tilePos + new Vector3(enemyGroupX, 0.0f, enemyGroupZ);

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
            Vector3 spawnPos = enemyGroupPos + new Vector3(socialDistancingX, 0.0f, socialDistancingZ);

            GameObject newEnemy = Instantiate(enemyPrefab, spawnPos, Quaternion.identity, entitiesParent);
            enemies.Add(newEnemy);
        }

        enemiesSpawned = true;
    }
}
