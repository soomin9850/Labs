using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.Tilemaps;
public class CharacterManager : MonoBehaviour
{
    public enum Type
    {
        rifle, search, machineGun, engineer, sniper, antiTank, mineThrower
    }
    public Type type;
    public string CharacterName;
    public Sprite Icon;
    public Sprite Stdillust;
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
            hp = value;
            HpBar.transform.localScale = new Vector2(BarValue(MaxHp, HP), 1);
            if (hp <= 0)
                die();
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
    [SerializeField] protected Tilemap BuildingTileMap;
    [SerializeField] protected List<Tile> Path;
    [SerializeField] protected List<Vector2> ClosePos = new List<Vector2>();
    [SerializeField] protected List<Vector2> MovePos = new List<Vector2>();
    [SerializeField] protected List<float> Value = new List<float>();
    public Vector2 TargetPos;
    public Coroutine MoveCoroutine;
    public Coroutine AttackCoroutine;
    int FIndCount;

    public MG MGStat;
    protected virtual void Awake()
    {
        HpBar = Bar.transform.GetChild(1).gameObject;
        HP = MaxHp;
        GM = GameManager.Instance;
        WallTileMap = GM.WallTile;
        BuildingTileMap = GM.PathFindTile;
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
            attackEnd = AIStart;
        }
    }
    void OnDisable()
    {
        StopAllCoroutines();
    }
    public void AIStart()
    {
        MovePos.Clear();
        ClosePos.Clear();
        Value.Clear();
        float NowX = Mathf.Round(transform.position.x);
        float NowY = Mathf.Round(transform.position.y);
        Vector2 NowPos = new Vector2(NowX, NowY);
        MovePos.Add(NowPos);
        if (MoveCoroutine != null)
            StopCoroutine(MoveCoroutine);
        FIndCount = 0;
        FindRoad();
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
        AIStart();
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
    IEnumerator StartMove()
    {
        if (MovePos.Count > 1)
        {
            if (AttackCoroutine != null)
                StopCoroutine(AttackCoroutine);
        }
        while (MovePos.Count > 0)
        {
            Vector2 Target = MovePos[0];
            while (Vector2.Distance(transform.position, Target) > 0.05f)
            {
                yield return null;
                transform.position = Vector2.MoveTowards(transform.position, Target, Speed * Time.deltaTime);
            }
            MovePos.RemoveAt(0);
        }
        if (TryGetComponent(out Tower tower))
        {
            tower.PlayerOrder = false;
        }
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
    void FindRoad()
    {
        FIndCount++;
        float NowX = Mathf.Round(MovePos[MovePos.Count - 1].x);
        float NowY = Mathf.Round(MovePos[MovePos.Count - 1].y);
        Vector2 NowPos = new Vector2(NowX, NowY);
        Vector2 Origin = new Vector2(transform.position.x, transform.position.y);
        if (Vector2.Distance(NowPos, TargetPos) < 1f)
        {
            MoveCoroutine = StartCoroutine(StartMove());
            return;
        }
        Vector2 Result = Vector2.zero;
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0)
                    continue;
                Result = NowPos + new Vector2(x, y);
                if (WallTileMap.GetTile(WallTileMap.WorldToCell(Result)) == null && BuildingTileMap.GetTile(WallTileMap.WorldToCell(Result)) == null && !MovePos.Contains(Result))
                {
                    ClosePos.Add(Result);
                    Value.Add((Vector2.Distance(Origin, NowPos) * 10) + (Vector2.Distance(Result, TargetPos) * 10));
                }
            }
        }
        if (Value.Count == 0 || FIndCount > 500)
        {
            Vector2 secondBest = Vector2.zero;
            for (int i = 0; i < MovePos.Count; i++)
            {
                if (Vector2.Distance(TargetPos, secondBest) > Vector2.Distance(TargetPos, MovePos[i]))
                    secondBest = MovePos[i];
            }
            int Index = MovePos.IndexOf(secondBest);
            while (MovePos.Count > Index + 1)
                MovePos.RemoveAt(Index + 1);
            MoveCoroutine = StartCoroutine(StartMove());
            return;
        }
        float min = Value.Min();
        List<int> Indexs = new List<int>();
        for (int i = 0; i < Value.Count; i++)
        {
            if (min == Value[i])
                Indexs.Add(i);
        }
        if (Indexs.Count > 1)
        {
            if ((int)(Vector2.Distance(ClosePos[Indexs[0]], TargetPos) * 10) > (int)(Vector2.Distance(ClosePos[Indexs[1]], TargetPos) * 10))
            {
                Value.RemoveAt(Indexs[0]);
                ClosePos.RemoveAt(Indexs[0]);
            }
            else
            {
                Value.RemoveAt(Indexs[1]);
                ClosePos.RemoveAt(Indexs[1]);
            }
        }
        MovePos.Add(ClosePos[Value.IndexOf(Value.Min())]);
        Value.Clear();
        ClosePos.Clear();
        if (Vector2.Distance(NowPos, TargetPos) > 0.05f && Target == null)
            FindRoad();
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
        B.Attack = Attack;
        B.TargetOBJ = Target;
        B.AntiAmor = AntiAmor;
        OBJ.SetActive(true);
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