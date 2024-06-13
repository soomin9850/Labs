using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : CharacterManager
{
    public bool PlayerOrder;
    WaitForSeconds WaitForSeconds = new WaitForSeconds(0.01f);
    public void Start()
    {
        AttackCoroutine = StartCoroutine(AttackThis(null));
        StopCoroutine(AttackCoroutine);
        Coru = false;
        die += TowerDie;
    }
    void OnDisable()
    {
        StopAllCoroutines();
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy") && !Coru && !PlayerOrder)
        {
            if (CheckWall(collision.gameObject) && !goingthere)
            {
                goingthere = true;
                TargetPos = collision.transform.position;
                StopCoroutine(MoveCoroutine);
                AIStart();
            }
        }
    }
    void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy")  && !Coru && !PlayerOrder)
        {
            if(CheckWall(collision.gameObject) && Vector2.Distance(collision.transform.position, transform.position) <= Sight)
            {
                StopCoroutine(MoveCoroutine);
                AttackCoroutine = StartCoroutine(AttackThis(collision.transform.gameObject));
            }
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
        while (Target != null)
        {
            Debug.Log(gameObject.name);
            if (CanAttack)
            {
                attackDel();
            }
            if (!CheckWall(Target))
            {
                Target = null;
            }
            yield return WaitForSeconds;
        }
        yield return null;
        RaycastHit2D hit2D = Physics2D.CircleCast(transform.position, Sight, transform.forward, 1, 1 << 7);
        if(hit2D.collider != null)
        {
            if (CheckWall(hit2D.transform.gameObject))
            {
                TargetPos = hit2D.transform.position;
                StopCoroutine(MoveCoroutine);
            }
        }
        Coru = false;
        AIStart();
        attackEnd();
    }
    public IEnumerator GoInBuilding(GameObject OBJ)
    {
        while (OBJ.TryGetComponent(out Building building) && !building.InPlayer)
        {
            if (Vector2.Distance(transform.position, OBJ.transform.position) < 3)
            {
                GameManager.Instance.InBuilding(building, this);
                break;
            }
            yield return null;
        }
        yield return null;
    }
    public void TowerDie()
    {
        gameObject.SetActive(false);
        GameManager.Instance.player.LoseTrigger.Remove(gameObject);
    }
}