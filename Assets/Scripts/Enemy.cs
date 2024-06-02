using System;
using System.Collections;
using UnityEngine;
public class Enemy : CharacterManager
{
    WaitForSeconds WaitForSeconds = new WaitForSeconds(0.01f);
    void Start()
    {
        WallTileMap = GM.WallTile;
        BuildingTileMap = GM.PathFindTile;
        AttackCoroutine = StartCoroutine(AttackThis(null));
        die += EnemyDie;
        StopCoroutine(AttackCoroutine);
        Invoke("WaitStart", 10);
    }
    void WaitStart()
    {
        AIStart();
    }
    void OnDisable()
    {
        StopAllCoroutines();
    }
    void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")  && Target == null)
        {
            if (CheckWall(collision.gameObject))
            {
                StopCoroutine(MoveCoroutine);
                AttackCoroutine = StartCoroutine(AttackThis(collision.transform.gameObject));
            }
        }
    }
    
    IEnumerator AttackThis(GameObject OBJ)
    {
        Target = OBJ;
        while (Target != null && Target.activeSelf)
        {
            if (Target.CompareTag("Building") && !Target.transform.GetComponent<Building>().InPlayer)
            {
                Target = null;
                break;
            }
            if (Vector2.Distance(Target.transform.position, transform.position) > Sight)
            {
                transform.position = Vector2.MoveTowards(transform.position, Target.transform.position, Speed * Time.deltaTime);
                yield return null;
                continue;
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
        attackEnd();
        AIStart();
    }
    public void EnemyDie()
    {
        Destroy(gameObject);
    }
}
