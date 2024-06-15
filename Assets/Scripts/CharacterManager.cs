using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
public class CharacterManager : MonoBehaviour
{
    public enum Type
    {
        rifle, search, machineGun, engineer, sniper, antiTank, mineThrower, tank
    }
    public Type type;
    public string CharacterName;
    public Sprite Icon;
    public Sprite Stdillust;
    public Sprite StatStdillust;
    protected GameManager GM;
    public int MaxHp;
    public GameObject Bar;
    GameObject HpBar;
    public int HP
    {
        get
        {
            return hp;
        }
        set
        {
            if (value > MaxHp)
                value = MaxHp;
            if (value <= 0)
            {
                value = 0;
                die();
            }
            hp = value;
            HpBar.transform.localScale = new Vector2(BarValue(MaxHp, HP), 1);
        }
    }
    [SerializeField] int hp;
    public GameObject Bullet;
    public float Sight = 5;
    public float Speed = 5;
    public float AttackSpeed = 1;
    public int Atk = 1;
    public int Amor;
    public float AntiAmor;
    protected bool NoDamage;
    [SerializeField] public bool CanAttack = true;

    public delegate void AttackDelegate();
    public AttackDelegate attackDel;
    public AttackDelegate attackEnd;

    public delegate void Die();
    public Die die;

    protected Coroutine Wait;
    public GameObject Target;

    [SerializeField] protected Tilemap WallTileMap;
    [SerializeField] protected Tilemap PathFindTileMap;
    [SerializeField] protected List<Vector2> MovePos = new List<Vector2>();
    public Vector2 TargetPos;
    public Coroutine MoveCoroutine;
    public Coroutine AttackCoroutine;
    int FIndCount;
    public List<Node> OpenNode = new List<Node>();
    public List<Node> CloseNode = new List<Node>();

    protected bool Coru;
    protected bool goingthere;
    public MG MGStat;

    Coroutine PathCo;
    protected virtual void Awake()
    {
        HpBar = Bar.transform.GetChild(1).gameObject;
        HP = MaxHp;
        GM = GameManager.Instance;
        WallTileMap = GM.WallTile;
        PathFindTileMap = GM.PathFindTile;
        Transform Child = transform.GetChild(transform.childCount - 1);
        Child.parent = null;
        float S = Sight * 2;
        Child.transform.localScale = new Vector3(S, S, S);
        Child.parent = transform;
        MoveCoroutine = StartCoroutine(StartMove());
        StopCoroutine(MoveCoroutine);
        if (type == Type.machineGun)
        {
            attackDel = MgAttack;
            attackEnd = MgAttackEnd;
        }
        else
        {
            attackDel = DefaultAttack;
            attackEnd = DefaultEnd;
        }
    }
    void OnDisable()
    {
        StopAllCoroutines();
    }
    public void DefaultEnd()
    {
        Debug.Log("Um");
    }
    public void AIStart()
    {
        TargetPos = new Vector2(Mathf.RoundToInt(TargetPos.x), Mathf.RoundToInt(TargetPos.y));
        OpenNode.Clear();
        CloseNode.Clear();
        MovePos.Clear();
        float NowX = transform.position.x;
        float NowY = transform.position.y;
        Vector2 NowPos = new Vector2(Mathf.RoundToInt(NowX), Mathf.RoundToInt(NowY));
        MovePos.Add(NowPos);
        if (MoveCoroutine != null)
            StopCoroutine(MoveCoroutine);
        FIndCount = 0;
        if (PathCo != null)
            StopCoroutine(PathCo);
        foreach(KeyValuePair<Vector2,Node> kv in GM.player.CustomTileData)
        {
            Node node = GM.player.CustomTileData[kv.Key];
            node.P = null;
            node.G = 0;
            node.H = 0;
        }
        PathCo = StartCoroutine(FindRoad());
    }
    public void Hit(int Attack, float AntiAmor)
    {
        if(NoDamage) return;
        int amor = (int)(Amor - (Amor * (AntiAmor * 0.01f)));
        if (amor < 0)
            amor = 0;
        int Damage = Attack - amor;
        if (TryGetComponent(out Building building))
        {
            building.BarrierHit(Damage);
            return;
        }
        HP -= Damage;
    }
    public void MgAttack()
    {
        CanAttack = false;
        BulletMake(Bullet, Target, Atk);
        if (MGStat.ShootTime > MGStat.Timer)
        {
            Wait = StartCoroutine(AttackOn(AttackSpeed));
            MGStat.Timer += AttackSpeed;
        }
        else
        {
            Wait = StartCoroutine(AttackOn(MGStat.ReLoadTime));
            MGStat.Timer = 0;
        }
    }
    public void MgAttackEnd()
    {
        if (Wait != null)
            StopCoroutine(Wait);
        CanAttack = false;
        Wait = StartCoroutine(AttackOn(MGStat.ReLoadTime - (int)MGStat.Timer * 0.5f));
        MGStat.Timer = 0;
    }
    public void DefaultAttack()
    {
        CanAttack = false;
        BulletMake(Bullet, Target, Atk);
        Wait = StartCoroutine(AttackOn(AttackSpeed));
    }
    protected IEnumerator AttackOn(float Time)
    {
        yield return new WaitForSeconds(Time);
        CanAttack = true;
    }
    void PathToMove(Node Start, Node End)
    {
        List<Vector2> path = new List<Vector2>();
        Node current = End;
        while (Vector2.Distance(current.Pos, Start.Pos) > 1f)
        {
            path.Add(current.Pos);
            current = current.P;
        }
        path.Reverse();
        MovePos = path.ToList();
        StopCoroutine(MoveCoroutine);
        MoveCoroutine = StartCoroutine(StartMove());
    }
    IEnumerator StartMove()
    {
        if (TryGetComponent(out Tower tower))
        {
            tower.PlayerOrder = false;
            yield return null;
            tower.PlayerOrder = true;
        }
        if (MovePos.Count > 1)
        {
            if (AttackCoroutine != null)
            {
                StopCoroutine(AttackCoroutine);
                Coru = false;
            }
        }
        while (MovePos.Count > 0)
        {
            float Timer = 0;
            Vector2 Target = MovePos[0];
            while (Vector2.Distance(transform.position, Target) > 0.1f)
            {
                yield return null;
                transform.position = Vector2.MoveTowards(transform.position, Target, Speed * Time.deltaTime);
                Timer += Time.deltaTime;
                if (Timer > 3)
                {
                    StopCoroutine(MoveCoroutine);
                    AIStart();
                }
            }
            transform.position = Target;
            MovePos.RemoveAt(0);
        }
        if (tower != null)
        {
            tower.PlayerOrder = false;
        }
        goingthere = false;
        yield return null;
    }
    protected bool CheckWall(GameObject target)
    {
        if(target == null)
            return false;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, (Vector2)target.transform.position - (Vector2)transform.position, Vector2.Distance((Vector2)target.transform.position, (Vector2)transform.position), 1 << 8);
        if (hit.collider == null)
            return true;
        return false;
    }
    IEnumerator FindRoad()
    {
        Node StartNode = GM.player.CustomTileData[MovePos[0]];
        Node Target = GM.player.CustomTileData[TargetPos];
        OpenNode.Add(StartNode);
        while (OpenNode.Count > 0)
        {
            Node current = OpenNode[0];
            for (int i = 1; i < OpenNode.Count; i++)
            {
                if (OpenNode[i].F < current.F || OpenNode[i].F == current.F && OpenNode[i].H < current.H)
                    current = OpenNode[i];
            }

            OpenNode.Remove(current);
            CloseNode.Add(current);
            Debug.Log("Count");
            FIndCount++;
            if (FIndCount == 10000)
            {
                if (GM.player.AIErrorCoroutine != null)
                    GM.player.StopCoroutine(GM.player.AIErrorCoroutine);
                GM.player.AIErrorCoroutine = GM.player.StartCoroutine(GM.player.AIError());
                yield break;
            }
            if (current == Target)
            {
                PathToMove(StartNode, current);
                yield break;
            }
            Vector2 Result = Vector2.zero;
            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    if (x == 0 && y == 0)
                        continue;
                    Result = current.Pos + new Vector2(x, y);
                    if (WallTileMap.GetTile(WallTileMap.WorldToCell(Result)) == null && PathFindTileMap.GetTile(PathFindTileMap.WorldToCell(Result)) == null && !CloseNode.Contains(GM.player.CustomTileData[Result]))
                    {
                        if (x == 1 && y == 1)
                        {
                            bool One = WallTileMap.GetTile(WallTileMap.WorldToCell(Result + new Vector2(-x, 0))) != null || PathFindTileMap.GetTile(PathFindTileMap.WorldToCell(Result + new Vector2(-x, 0))) != null;
                            bool Two = WallTileMap.GetTile(WallTileMap.WorldToCell(Result + new Vector2(0, -y))) != null || PathFindTileMap.GetTile(PathFindTileMap.WorldToCell(Result + new Vector2(0, -y))) != null;
                            if (One && Two)
                                continue;
                        }
                        else if (x == -1 && y == 1)
                        {
                            bool One = WallTileMap.GetTile(WallTileMap.WorldToCell(Result + new Vector2(-x, 0))) != null || PathFindTileMap.GetTile(PathFindTileMap.WorldToCell(Result + new Vector2(-x, 0))) != null;
                            bool Two = WallTileMap.GetTile(WallTileMap.WorldToCell(Result + new Vector2(0, -y))) != null || PathFindTileMap.GetTile(PathFindTileMap.WorldToCell(Result + new Vector2(0, -y))) != null;
                            if (One && Two)
                                continue;
                        }
                        else if (x == 1 && y == -1)
                        {
                            bool One = WallTileMap.GetTile(WallTileMap.WorldToCell(Result + new Vector2(-x, 0))) != null || PathFindTileMap.GetTile(PathFindTileMap.WorldToCell(Result + new Vector2(-x, 0))) != null;
                            bool Two = WallTileMap.GetTile(WallTileMap.WorldToCell(Result + new Vector2(0, -y))) != null || PathFindTileMap.GetTile(PathFindTileMap.WorldToCell(Result + new Vector2(0, -y))) != null;
                            if (One && Two)
                                continue;
                        }
                        else if (x == -1 && y == -1)
                        {
                            bool One = WallTileMap.GetTile(WallTileMap.WorldToCell(Result + new Vector2(-x, 0))) != null || PathFindTileMap.GetTile(PathFindTileMap.WorldToCell(Result + new Vector2(-x, 0))) != null;
                            bool Two = WallTileMap.GetTile(WallTileMap.WorldToCell(Result + new Vector2(0, -y))) != null || PathFindTileMap.GetTile(PathFindTileMap.WorldToCell(Result + new Vector2(0, -y))) != null;
                            if (One && Two)
                                continue;
                        }
                        Node NextNode = GM.player.CustomTileData[Result];
                        float NewG = current.G + Vector2.Distance(current.Pos, Result);
                        if (NewG < NextNode.G || !OpenNode.Contains(NextNode))
                        {
                            NextNode.G = NewG;
                            NextNode.H = Vector2.Distance(NextNode.Pos, TargetPos);
                            NextNode.P = current;
                            if (!OpenNode.Contains(NextNode))
                            {
                                OpenNode.Add(NextNode);
                            }
                        }
                    }
                }
            }
        }
        yield return null;
    }
    float Distance(Vector2 A, Vector2 B)
    {
        float X = A.x - B.x;
        float Y = A.y - B.y;
        if (X < 0) X *= -1;
        if (Y < 0) Y *= -1;
        if (X > Y)
            return 14 * X + 10 * (X - Y);
        return 14 * X + 10 * (Y - X);
    }
    protected float BarValue(int Max, int Now)
    {
        return (float)Now / (float)Max;
    }
    protected void BulletMake(GameObject bullet, GameObject Target,int Attack)
    {
        int BulletType = bullet.GetComponent<Bullet>().Type;
        GameObject OBJ = null;
        switch (BulletType)
        {
            case 0:
                OBJ = ZeroPool();
                break;
            case 1:
                OBJ = OnePool();
                break;
        }
        if (OBJ == null)
            OBJ = Instantiate(bullet);
        OBJ.transform.position = transform.position;
        Bullet B = OBJ.GetComponent<Bullet>();
        B.Target = Target;
        B.TargetOBJ = Target;
        B.AntiAmor = AntiAmor;
        float Atk = Attack;
        CharacterManager TargetCM = Target.transform.GetComponent<CharacterManager>();
        if (TargetCM.type == Type.sniper)
        {
            if (type == Type.sniper)
                CounterAttack(ref Atk);
            else if (type == Type.search)
                CounterAttack(ref Atk);
        }
        else if(TargetCM.type == Type.search)
        {
            if(type == Type.rifle)
                CounterAttack(ref Atk);
        }
        else if (TargetCM.type == Type.rifle)
        {
            if (type == Type.tank)
                CounterAttack(ref Atk);
        }
        else if (TargetCM.type == Type.tank)
        {
            if (type == Type.antiTank)
                CounterAttack(ref Atk);
        }
        B.Attack = Mathf.RoundToInt(Atk);
        OBJ.SetActive(true);
    }
    void CounterAttack(ref float Attack)
    {
        Attack *= 1.1f;
    }
    GameObject ZeroPool()
    {
        if (GM.bulletZero.Count > 0)
            return GM.bulletZero.Dequeue();
        return null;
    }
    GameObject OnePool()
    {
        if (GM.bulletOne.Count > 0)
            return GM.bulletOne.Dequeue();
        return null;
    }
}