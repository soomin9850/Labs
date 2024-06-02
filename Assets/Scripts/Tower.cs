using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : CharacterManager
{
    [SerializeField] bool CanAttack = true;
    public bool PlayerOrder;
    Coroutine Wait;
    WaitForSeconds WaitForSeconds = new WaitForSeconds(0.01f);
    void Start()
    {
        AttackCoroutine = StartCoroutine(AttackThis(null));
        StopCoroutine(AttackCoroutine);
    }
    void OnDisable()
    {
        StopAllCoroutines();
    }
    void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy")  && Target == null && !PlayerOrder)
        {
            if(CheckWall(collision.gameObject))
            {
                StopCoroutine(MoveCoroutine);
                AttackCoroutine = StartCoroutine(AttackThis(collision.transform.gameObject));
            }
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
        while (Target != null)
        {
            Debug.Log(gameObject.name);
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
                    if(MGStat.ShootTime > MGStat.Timer)
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
        AIStart();
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
    public void Die()
    {
        gameObject.SetActive(false);
    }
    IEnumerator AttackOn(float Time)
    {
        yield return new WaitForSeconds(Time);
        CanAttack = true;
    }
}