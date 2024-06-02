using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using static CharacterManager;

public class Car : MonoBehaviour
{
    [SerializeField] int Width;
    [SerializeField] int Height;
    [SerializeField] Tilemap PathFindTile;
    [SerializeField] TileBase TileBase;
    public bool isMove;
    void Start()
    {
        PathFindTile = GameManager.Instance.PathFindTile;
        Set();
    }
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Bullet"))
        {
            collision.GetComponent<Bullet>().BulletPooling();
        }
    }
    public void Move(Vector2 Dir)
    {
        if (isMove)
            return;
        isMove = true;
        Clear();
        Vector2 Target = (Vector2)transform.position + Dir;
        StartCoroutine(Go(Target));
    }
    IEnumerator Go(Vector2 Target)
    {
        yield return null;
        bool CanMove = true;
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                Vector3Int vector3 = new Vector3Int((int)Target.x + x, (int)Target.y + y);
                if (PathFindTile.GetTile(vector3) != null)
                    CanMove = false;
            }
        }
        if(CanMove)
        {
            while (Vector2.Distance(transform.position, Target) > 0.05f)
            {
                transform.position = Vector2.MoveTowards(transform.position, Target, 5 * Time.deltaTime);
                yield return null;
            }
        }
        transform.position = Target;
        yield return null;
        isMove = false;
        Set();
    }
    public void Set()
    {
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                Vector3Int vector3 = new Vector3Int(Mathf.RoundToInt(transform.position.x) + x, Mathf.RoundToInt(transform.position.y) + y);
                PathFindTile.SetTile(vector3, TileBase);
            }
        }
    }
    public void Clear()
    {
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                Vector3Int vector3 = new Vector3Int(Mathf.RoundToInt(transform.position.x) + x, Mathf.RoundToInt(transform.position.y) + y);
                PathFindTile.SetTile(vector3, null);
            }
        }
    }
}
