using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Building : CharacterManager
{
    public float BuildingSize = 0;
    SpriteRenderer SR;
    Color Half = new Color(1, 1, 1, 0.5f);
    [SerializeField] int MaxBarrier;
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
        AttackCoroutine = StartCoroutine(AttackThis(null));
        die += BuildingDie;
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
        if (type == Type.machineGun)
        {
            if (Wait != null)
                StopCoroutine(Wait);
            CanAttack = false;
            Wait = StartCoroutine(AttackOn(MGStat.ReLoadTime - (int)MGStat.Timer * 0.5f));
            MGStat.Timer = 0;
        }
    }
    public void BuildingDie()
    {
        if(InPlayer)
        {
            CharacterManager character = transform.GetChild(transform.childCount - 1).GetComponent<CharacterManager>();
            GameManager.Instance.OutBuilding(GetComponent<CharacterManager>(), character);
            character.die();
        }
        this.enabled = false;
        //GetComponent<SpriteRenderer>().sprite = ±×°Å;
        NoDamage = true;
    }
}