using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public GameObject TargetOBJ;
    [SerializeField] float BulletSpeed = 5;
    public int Type = 0;
    public int Attack;
    public GameObject Target;
    public float AntiAmor;
    void Update()
    {
        if (Target == null || !Target.activeSelf)
        {
            BulletPooling(gameObject);
            return;
        }
        transform.position = Vector2.MoveTowards(transform.position, Target.transform.position, BulletSpeed * Time.deltaTime);
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject == TargetOBJ || collision.CompareTag("Building"))
        {
            collision.GetComponent<CharacterManager>().Hit(Attack, AntiAmor);
            BulletPooling(gameObject);
        }
    }
    void ZeroPool(GameObject bullet)
    {
        GameManager.Instance.bulletZero.Enqueue(bullet);
        bullet.SetActive(false);
    }
    void OnePool(GameObject bullet)
    {
        GameManager.Instance.bulletOne.Enqueue(bullet);
        bullet.SetActive(false);
    }
    void BulletPooling(GameObject bullet)
    {
        int BulletType = bullet.GetComponent<Bullet>().Type;
        switch (BulletType)
        {
            case 0:
                ZeroPool(bullet);
                break;
            case 1:
                OnePool(bullet);
                break;
        }
    }
}