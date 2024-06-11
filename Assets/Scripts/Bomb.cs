using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    public GameObject Target;
    public void Click()
    {
        if(Target.TryGetComponent(out Bridge bridge))
        {
             //x크기y크기로 for 타일맵 넣기함수 호출
        }
        else if(Target.TryGetComponent(out Building building))
        {
            building.BuildingDie();
        }
        RaycastHit2D[] hits = Physics2D.CircleCastAll(Camera.main.ScreenToWorldPoint(Input.mousePosition), 2.5f, transform.forward, 1, 1 << 6 | 1 << 7);
        for(int i = 0; i < hits.Length; i++) 
        {
            hits[i].transform.GetComponent<CharacterManager>().HP -= 200;
        }
    }
}
