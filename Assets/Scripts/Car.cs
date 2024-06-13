using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Car : MonoBehaviour
{
    [SerializeField] int Width;
    [SerializeField] int Height;
    [SerializeField] Tilemap PathFindTile;
    [SerializeField] Tilemap WallTile;
    [SerializeField] TileBase TileBase;
    [SerializeField] int Rz = 0;
    public int MaxHP;
    public int HP;
    public bool isMove;
    void Start()
    {
        PathFindTile = GameManager.Instance.PathFindTile;
        WallTile = GameManager.Instance.WallTile;
        HP = MaxHP;
        Set();
    }
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Bullet"))
        {
            Bullet B = collision.GetComponent<Bullet>();
            B.BulletPooling();
            HP -= B.Attack;
            if (HP < 0)
            {
                HP = 0;
                transform.GetChild(0).GetComponent<BoxCollider2D>().enabled = false;
                transform.GetChild(0).GetComponent<SpriteRenderer>().color = Color.red;
            }
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
                if (PathFindTile.GetTile(Position(x, y)) != null || WallTile.GetTile(Position(x, y)) != null)
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
            transform.position = Target;
        }
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
    Vector3Int Position(int x, int y)
    {
        Vector3Int vector3;
        if ((Rz == -45f || Rz == 135f) && x == 1)
            vector3 = new Vector3Int(Mathf.RoundToInt(transform.position.x) + x, Mathf.RoundToInt(transform.position.y) + 1);
        else if ((Rz == 45f || Rz == -135f) && x == 1)
            vector3 = new Vector3Int(Mathf.RoundToInt(transform.position.x) + x, Mathf.RoundToInt(transform.position.y) - 1);
        else
            vector3 = new Vector3Int(Mathf.RoundToInt(transform.position.x) + x, Mathf.RoundToInt(transform.position.y) + y);
        return vector3;
    }
}
