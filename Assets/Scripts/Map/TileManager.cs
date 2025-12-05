using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class TileManager : MonoBehaviour
{
    [SerializeField] private BaseTile tilePrefab;

    [SerializeField] private int viewDistance = 1;

    [SerializeField] private GameObject player;

    private Dictionary<Vector2Int, BaseTile> tiles = new();
    private Vector2Int currentTile;

    public Dictionary<Vector2Int, BaseTile> GetActiveTiles() {  return tiles; }
    public Vector2Int GetCurrentTile() { return currentTile; }

    private void Start()
    {
        UpdateTiles(true);
    }

    private void Update()
    {
        UpdateTiles();
    }

    private void UpdateTiles(bool firstTile = false)
    {
        if (player == null) return;

        Vector2Int newCurrentTile = GetPlayerTile();
        if (newCurrentTile == currentTile && !firstTile) return;
        currentTile = newCurrentTile;

        for (int x = -viewDistance; x <= viewDistance; x++)
        {
            for (int y = -viewDistance; y <= viewDistance; y++)
            {
                Vector2Int tileCoord = new Vector2Int(currentTile.x + x, currentTile.y + y);
                
                if (tiles.TryGetValue(tileCoord, out BaseTile tile))
                {
                    if (!tile.gameObject.activeSelf)
                        tile.gameObject.SetActive(true);
                } else {
                    GenerateTile(tileCoord);
                }
            }
        }

        foreach (var tilePair in tiles)
        {
            Vector2Int tileCoord = tilePair.Key;
            BaseTile tile = tilePair.Value;

            int absX = Mathf.Abs(tileCoord.x - currentTile.x);
            int absY = Mathf.Abs(tileCoord.y - currentTile.y);

            if (absX > viewDistance || absY > viewDistance)
            {
                if (tile.gameObject.activeSelf)
                    tile.gameObject.SetActive(false);
            }
        }

    }

    private void GenerateTile(Vector2Int gridCoord)
    {
        BaseTile tile = Instantiate(tilePrefab, Vector3.zero, Quaternion.identity, transform);
        tile.Init(gridCoord);
        tiles.Add(gridCoord, tile);
    }

    private Vector2Int GetPlayerTile()
    {
        if (tilePrefab == null) return Vector2Int.zero;

        Vector2 tileSize = tilePrefab.GetTileSize();
        int x = Mathf.FloorToInt(player.transform.position.x / tileSize.x);
        int y = Mathf.FloorToInt(player.transform.position.z / tileSize.y);

        return new Vector2Int(x, y);
    }

    public string GetPlayerTileTag()
    {
        Vector2Int currentTile = GetCurrentTile();
        if (tiles.TryGetValue(currentTile, out BaseTile tile))
        {
            return tile.tag;
        }
        return "";
    }
}
