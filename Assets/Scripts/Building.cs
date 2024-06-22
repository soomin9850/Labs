using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Building : CharacterManager
{
    public int BuildingSize = 0;
    SpriteRenderer SR;
    [SerializeField] Sprite Destroyed;
    [SerializeField] GameObject BuildingWreck;
    Color Half = new Color(1, 1, 1, 0.5f);
    [SerializeField] int MaxBarrier;
    public bool BombReady;
    public bool Trench;
    public int Barrier
    {
        get
        {
            return barrier;
        }
        set
        {
            if (value > MaxBarrier) value = MaxBarrier;
            barrier = value;
            BarrierBar.transform.localScale = new Vector2(BarValue(MaxBarrier, Barrier), 1);
        }
    }
    [SerializeField] int barrier;
    GameObject BarrierBar;

    public Vector3 ExitDir = Vector3.zero;
    public bool InPlayer;
    WaitForSeconds WaitForSeconds = new WaitForSeconds(0.01f);
    protected override void Awake()
    {
        base.Awake();
        SR = GetComponent<SpriteRenderer>();
        BarrierBar = Bar.transform.GetChild(0).gameObject;
        Barrier = MaxBarrier;
        Bar.SetActive(false);
    }
    void OnDisable()
    {
        StopAllCoroutines();
    }
    void Start()
    {
        PathFindTileMap = GM.PathFindTile;
        WallTileMap = GM.WallTile;
        if (Trench)
        {
            Vector3Int vector3 = new Vector3Int(Mathf.RoundToInt(transform.position.x - 0.5f), Mathf.RoundToInt(transform.position.y - 0.5f));
            transform.position = vector3;
            PathFindTileMap.SetTile(vector3, GameManager.Instance.TileBase);
        }
        AttackCoroutine = StartCoroutine(AttackThis(null));
        die += BuildingDie;
        StopCoroutine(AttackCoroutine);
        Coru = false;
    }
    public void BarrierHit(int Atk)
    {
        Barrier -= Atk;
        if(Barrier <= 0)
        {
            HP -= -Barrier;
            Barrier = 0;
        }
    }
    void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy") && !Coru && InPlayer)
        {
            Building_CheckWall(collision.gameObject);
        }
        else if (Coru && Target == null)
            Coru = false;
        else if (collision.CompareTag("Player"))
            SR.color = Half;
    }
    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
            SR.color = Color.white;
    }
    void Building_CheckWall(GameObject target)
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, (Vector2)target.transform.position - (Vector2)transform.position, Vector2.Distance((Vector2)target.transform.position, (Vector2)transform.position), 1 << 8);
        if (hit.collider == null)
        {
            AttackCoroutine = StartCoroutine(AttackThis(target.transform.gameObject));
        }

    }
    public void ThatAttack(GameObject Target)
    {
        if (Vector2.Distance(Target.transform.position, transform.position) <= Sight)
            AttackCoroutine = StartCoroutine(AttackThis(Target));
    }
    IEnumerator AttackThis(GameObject OBJ)
    {
        Target = OBJ;
        Coru = true;
        if (!CheckWall(Target))
        {
            Target = null;
        }
        while (Target != null && InPlayer)
        {
            if (CanAttack)
            {
                attackDel();
            }
            if (Vector2.Distance(Target.transform.position, transform.position) > Sight + 5f)
                Target = null;
            if (!CheckWall(Target))
            {
                Target = null;
            }
            yield return WaitForSeconds;
        }
        yield return null;
        RaycastHit2D hit2D = Physics2D.CircleCast(transform.position, Sight, transform.forward, 1, 1 << 7);
        if (hit2D.collider != null)
        {
            Target = hit2D.transform.gameObject;
        }
        attackEnd();
        Coru = false;
    }
    public void BuildingDie()
    {
        if(InPlayer)
        {
            CharacterManager character = transform.GetChild(transform.childCount - 1).GetComponent<CharacterManager>();
            GameManager.Instance.OutBuilding(GetComponent<CharacterManager>(), character);
            character.die();
        }
        SR.sprite = Destroyed;
        List<Vector2> list = new List<Vector2>();
        for (int x = 0; x <= 5; x++)
        {
            for (int y = 0; y <= 5; y++)
            {
                Vector2 RandomList = new Vector2(Mathf.RoundToInt(transform.position.x) + x, Mathf.RoundToInt(transform.position.y) + y);
                if (PathCheck(RandomList))
                {
                    list.Add(RandomList);
                }
            }
        }
        for (int x = 0; x >= -5; x--)
        {
            for (int y = 0; y >= -5; y--)
            {
                Vector2 RandomList = new Vector2(Mathf.RoundToInt(transform.position.x) + x, Mathf.RoundToInt(transform.position.y) + y);
                if (PathCheck(RandomList))
                {
                    list.Add(RandomList);
                }
            }
        }
        int Repeat = BuildingSize + 1;
        while (Repeat > 0)
        {
            int i = Random.Range(0, list.Count);
            list.RemoveAt(i);
            Instantiate(BuildingWreck, list[i], Quaternion.identity);
            Repeat--;
        }
        Destroy(this);
    }
    bool PathCheck(Vector2 RandomList)
    {
        if (PathFindTileMap.GetTile(PathFindTileMap.WorldToCell(RandomList)) == null)
        {
            return true;
        }
        return false;
    }
}