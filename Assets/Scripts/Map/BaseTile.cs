using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseTile : MonoBehaviour
{
    private Vector2Int gridCoord;
    private bool isInit = false;

    public Vector2 GetTileSize()
    {
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null )
        {
            return new Vector2(renderer.bounds.size.x, renderer.bounds.size.z);
        } else
        {
            return Vector2.one;
        }
    }
    public Vector2Int GetGridCoord() { return gridCoord; }
    public bool GetIsInit() { return isInit; }

    public virtual void Init(Vector2Int coord)
    {
        if (isInit) return;

        Renderer renderer = GetComponent<Renderer>();
        if (renderer == null) return;

        Vector2 tileSize = GetTileSize();

        gridCoord = coord;
        transform.position = new Vector3(coord.x * tileSize.x, 0.0f, coord.y * tileSize.y);
        name = $"Tile@{coord.x},{coord.y}";

        isInit = true;
    }
}
