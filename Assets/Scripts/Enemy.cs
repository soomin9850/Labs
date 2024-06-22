using System;
using System.Collections;
using UnityEngine;
public class Enemy : CharacterManager
{
    WaitForSeconds WaitForSeconds = new WaitForSeconds(0.01f);
    void Start()
    {
        WallTileMap = GM.WallTile;
        PathFindTileMap = GM.PathFindTile;
        AttackCoroutine = StartCoroutine(AttackThis(null));
        die += EnemyDie;
        StopCoroutine(AttackCoroutine);
        Coru = false;
        TargetPos = GM.player.Home.transform.position;
        AIStart();
    }
    public void WaitStart()
    {
        AIStart();
    }
    void OnDisable()
    {
        StopAllCoroutines();
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !Coru)
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
        if (collision.CompareTag("Player") && !Coru)
        {
            if (CheckWall(collision.gameObject) && Vector2.Distance(collision.transform.position, transform.position) <= Sight)
            {
                StopCoroutine(MoveCoroutine);
                AttackCoroutine = StartCoroutine(AttackThis(collision.transform.gameObject));
            }
        }
        else if (Coru && Target == null)
            Coru = false;
    }
    
    IEnumerator AttackThis(GameObject OBJ)
    {
        Target = OBJ;
        Coru = true;
        while (Target != null && Target.activeSelf)
        {
            if (Target.CompareTag("Building") || (Target.transform.TryGetComponent(out Building building) && !building.InPlayer))
            {
                Target = null;
                break;
            }
            else if (CanAttack)
            {
                attackDel();
            }
            if (!CheckWall(Target))
                Target = null;
            if (Target != null && Vector2.Distance(Target.transform.position, transform.position) > Sight)
                Target = null;
            yield return WaitForSeconds;
        }
        Target = null;
        yield return null;
        TargetPos = GameManager.Instance.player.Home.transform.position;
        RaycastHit2D hit2D = Physics2D.CircleCast(transform.position, Sight, transform.forward, 1, 1 << 6);
        if (hit2D.collider != null)
        {
            if (CheckWall(hit2D.transform.gameObject))
            {
                TargetPos = hit2D.transform.position;
                StopCoroutine(MoveCoroutine);
            }
        }
        Coru = false;
        TargetPos = GM.player.Home.transform.position;
        AIStart();
        attackEnd();
    }
    public void EnemyDie()
    {
        Destroy(gameObject);
    }
}
