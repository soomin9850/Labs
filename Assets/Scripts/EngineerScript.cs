using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EngineerScript : MonoBehaviour
{
    Tower tower;
    void Start()
    {
        tower = GetComponent<Tower>();
    }
    public void MakeBomb(GameObject OBJ)
    {
        Debug.Log("A");
        if (OBJ.TryGetComponent(out Building building))
        {
            Debug.Log("W");
            StartCoroutine(BuildWait(building));
        }
    }
    public void SummonBomb(GameObject Target)
    {
        GameManager.Instance.player.BombCount++;
        GameObject Bomb = Instantiate(GameManager.Instance.player.Bomb, Target.transform.position, Quaternion.identity);
        Bomb.GetComponent<Bomb>().Target = Target;
    }
    IEnumerator BuildWait(Building Building)
    {
        yield return null;
        while (tower.PlayerOrder)
        {
            yield return null;
        }
        if(Vector2.Distance(Building.transform.position, transform.position) > 5f)
        {
            yield break;
        }
        int a = 10 + (Building.BuildingSize * 5);
        float Timer = 0;
        tower.StopCoroutine(tower.MoveCoroutine);
        tower.PlayerOrder = true;
        while (tower.PlayerOrder && Timer < a) 
        {
            Timer += Time.deltaTime;
            Debug.Log(Timer);
            yield return null;
        }
        if (Timer >= a)
            SummonBomb(Building.gameObject);
        tower.PlayerOrder = false;
        yield return null;
    }
}
