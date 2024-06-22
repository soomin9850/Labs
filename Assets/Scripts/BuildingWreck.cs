using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BuildingWreck : MonoBehaviour
{
    Tilemap PathFindTile;
    GameManager GM;
    [SerializeField] int Width;
    [SerializeField] int Height;
    [SerializeField] TileBase TileBase;
    void Start()
    {
        GM = GameManager.Instance;
        PathFindTile = GameManager.Instance.PathFindTile;
        transform.position = Position();
        Set();
    }
    public void Set()
    {
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                PathFindTile.SetTile(Position(x, y), TileBase);
            }
        }
    }
    public void Clear()
    {
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                PathFindTile.SetTile(Position(x, y), null);
            }
        }
    }
    Vector3Int Position(int x = 0, int y = 0)
    {
        return new Vector3Int(Mathf.RoundToInt(transform.position.x) + x, Mathf.RoundToInt(transform.position.y) + y);
    }
}
