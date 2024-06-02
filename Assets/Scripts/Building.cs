using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Building : CharacterManager
{
    public float BuildingSize = 0;
    [SerializeField] int MaxBarrier;
    Coroutine Wait;
    int Barrier
    {
        get
        {
            return barrier;
        }
        set
        {
            barrier = value;
            BarrierBar.transform.localScale = new Vector2(BarValue(MaxBarrier, Barrier), 1);
        }
    }
    [SerializeField] int barrier;
    GameObject BarrierBar;

    [SerializeField] bool CanAttack = true;
    public Vector3 ExitDir = Vector3.zero;
    public bool InPlayer;
    WaitForSeconds WaitForSeconds = new WaitForSeconds(0.01f);
    protected override void Awake()
    {
        base.Awake();
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
        AttackCoroutine = StartCoroutine(AttackThis(null));
        StopCoroutine(AttackCoroutine);
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
        if (collision.CompareTag("Enemy") && Target == null && InPlayer)
        {
            Building_CheckWall(collision.gameObject);
        }
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
        AttackCoroutine = StartCoroutine(AttackThis(Target));
    }
    IEnumerator AttackThis(GameObject OBJ)
    {
        Target = OBJ;
        if (!CheckWall(Target))
        {
            Target = null;
        }
        while (Target != null && InPlayer)
        {
            Debug.Log(gameObject.name);
            if (CanAttack)
            {
                CanAttack = false;
                BulletMake(Bullet, Target, Atk);
                if (type == Type.machineGun)
                {
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
                else
                    Wait = StartCoroutine(AttackOn(AttackSpeed));
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
        if (type == Type.machineGun)
        {
            Debug.Log("End");
            if (Wait != null)
                StopCoroutine(Wait);
            CanAttack = false;
            Wait = StartCoroutine(AttackOn(MGStat.ReLoadTime - (int)MGStat.Timer * 0.5f));
            MGStat.Timer = 0;
        }
    }
    public void Die()
    {
        GameManager.Instance.OutBuilding(GetComponent<CharacterManager>(), transform.GetChild(transform.childCount - 1).GetComponent<CharacterManager>());
        NoDamage = true;
    }
    IEnumerator AttackOn(float Time)
    {
        yield return new WaitForSeconds(Time);
        CanAttack = true;
    }
}