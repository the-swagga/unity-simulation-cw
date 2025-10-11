using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GrasslandsTile : BaseTile
{
    [SerializeField] private float treeDensity = 0.05f;
    [SerializeField] private Vector3 treeScale = Vector3.one;
    [SerializeField] private float waterProb = 0.1f;
    [SerializeField] private GameObject[] treePrefabs;
    [SerializeField] private GameObject[] enemyPrefabs;
    [SerializeField] private GameObject[] itemPrefabs;

    public override void Init(Vector2Int coord)
    {
        base.Init(coord);
        PopulateTile();
    }

    private void PopulateTile()
    {
        GenerateTrees();
        GenerateWater();
    }

    private void GenerateTrees()
    {
        if (treePrefabs.Length == 0) return;

        Vector2 tileSize = GetTileSize();
        int treeCount = Mathf.RoundToInt(tileSize.x * treeDensity);

        for (int i = 0; i < treeCount; i++)
        {
            int randomTreeIndex = Random.Range(0, treePrefabs.Length);
            GameObject treePrefab = treePrefabs[randomTreeIndex];

            float randomX = Random.Range(-tileSize.x / 2.0f, tileSize.x / 2.0f);
            float randomZ = Random.Range(-tileSize.y / 2.0f, tileSize.y / 2.0f);
            Vector3 treePos = transform.position + new Vector3(randomX, 0, randomZ);
            Quaternion treeRot = Quaternion.Euler(0, Random.Range(0, 360), 0);

            GameObject tree = Instantiate(treePrefab, treePos, treeRot, transform);
            tree.transform.localScale = treeScale;
        }
    }

    private void GenerateWater()
    {

    }
}
