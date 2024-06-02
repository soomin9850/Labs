using System;
using System.Collections;
using UnityEngine;
public class Enemy : CharacterManager
{
    bool CanAttack = true;
    Coroutine Wait;
    WaitForSeconds WaitForSeconds = new WaitForSeconds(0.01f);
    void Start()
    {
        WallTileMap = GM.WallTile;
        BuildingTileMap = GM.BuildingTile;
        AttackCoroutine = StartCoroutine(AttackThis(null));
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
            if (!CheckWall(Target))
                Target = null;
            if (Target != null && Vector2.Distance(Target.transform.position, transform.position) > Sight)
                Target = null;
            yield return WaitForSeconds;
        }
        Target = null;
        yield return null;
        if (type == Type.machineGun)
        {
            if (Wait != null)
                StopCoroutine(Wait);
            CanAttack = false;
            Wait = StartCoroutine(AttackOn(MGStat.ReLoadTime - (int)MGStat.Timer * 0.5f));
            MGStat.Timer = 0;
        }
        AIStart();
    }
    public void Die()
    {
        Destroy(gameObject);
    }
    IEnumerator AttackOn(float Time)
    {
        yield return new WaitForSeconds(Time);
        CanAttack = true;
    }
}
