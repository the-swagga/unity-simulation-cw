using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GrasslandsTile : BaseTile
{
    [SerializeField] private float treeDensity = 0.05f;
    [SerializeField] private Vector3 treeScale = Vector3.one;
    [SerializeField] private float waterProb = 0.1f;
    [SerializeField] private Vector3 waterScale = Vector3.one;
    [SerializeField] private Vector2 waterScaleRange = new Vector2(0.1f, 1.0f);
    [SerializeField] private float waterYPos = 0.0f;
    [SerializeField] private GameObject waterPrefab;
    [SerializeField] private GameObject[] treePrefabs;
    [SerializeField] private GameObject[] enemyPrefabs;
    [SerializeField] private GameObject[] itemPrefabs;
    [SerializeField] private float treeGrowthRate;
    [SerializeField] private float treeMaxScale;

    private List<GameObject> trees = new List<GameObject>();

    public override void Init(Vector2Int coord)
    {
        base.Init(coord);
        PopulateTile();
    }

    private void Update()
    {
        GrowTrees();
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
            trees.Add(tree);
            Vector3 tileScale = transform.localScale;
            tree.transform.localScale = new Vector3(treeScale.x / tileScale.x, treeScale.y / tileScale.y, treeScale.z / tileScale.z);
        }
    }

    private void GenerateWater()
    {
        if (waterPrefab == null) return;

        Vector2 tileSize = GetTileSize();
        float randomNum = Random.Range(0.0f, 1.0f);
        if (randomNum <= waterProb)
        {
            float randomX = Random.Range(-tileSize.x / 2.0f, tileSize.x / 2.0f);
            float randomZ = Random.Range(-tileSize.y / 2.0f, tileSize.y / 2.0f);
            Vector3 waterPos = transform.position + new Vector3(randomX, waterYPos, randomZ);

            GameObject water = Instantiate(waterPrefab, waterPos, Quaternion.identity, transform);
            Vector3 tileScale = transform.localScale;
            float randomScale = Random.Range(waterScaleRange.x, waterScaleRange.y);
            water.transform.localScale = new Vector3((waterScale.x * randomScale) / tileScale.x, waterScale.y / tileScale.y, (waterScale.z * randomScale) / tileScale.z);
        }
    }

    private void GrowTrees()
    {
        foreach (GameObject tree in trees)
        {
            if (tree == null) continue;

            if (tree.transform.localScale.x < (treeScale.x * treeMaxScale)) tree.transform.localScale *= treeGrowthRate;
        }
    }
}
