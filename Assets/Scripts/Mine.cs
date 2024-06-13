using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mine : MonoBehaviour
{
    public enum Type
    {
        AntiTank,AntiPerson
    }
    public Type type;
    delegate void OnTrigger(GameObject gameObject);
    OnTrigger onTrigger;
    public string TargetTag = "Enemy";
    void Start()
    {
        if (type == Type.AntiTank)
        {
            onTrigger = Tank;
        }
        else if (type == Type.AntiPerson)
        {
            onTrigger = Person;
        }
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag(TargetTag))
        {
            onTrigger(collision.gameObject);
        }
    }
    void Tank(GameObject OBJ)
    {
        if (OBJ.TryGetComponent(out CharacterManager CM) && CM.type == CharacterManager.Type.tank)
        {
            CM.HP -= 70;
            Destroy(gameObject);
        }
    }
    void Person(GameObject OBJ)
    {
        if (OBJ.TryGetComponent(out CharacterManager CM))
        {
            if (CM.type == CharacterManager.Type.tank)
            {
                CM.HP -= 0;
                Destroy(gameObject);
            }
            else
            {
                CM.HP -= 30;
                Destroy(gameObject);
            }
        }
    }
}
