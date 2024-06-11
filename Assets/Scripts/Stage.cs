using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage : MonoBehaviour
{
    public int EnemyCount;
    [SerializeField] GameObject[] Enemy;
    [SerializeField] Vector2[] SummonPos;
    Coroutine Coroutine;
    void Start()
    {
        Coroutine = StartCoroutine(StageWait(10));
    }
    IEnumerator StageWait(float Time)
    {
        int index = 0;
        yield return new WaitForSeconds(Time);
        for (int i = 0; i < 10; i++)
        {
            yield return new WaitForSeconds(0.1f);
            GameObject OBJ = Instantiate(Enemy[0], SummonPos[index], Quaternion.identity);
            OBJ.GetComponent<CharacterManager>().TargetPos = GameManager.Instance.player.Home.transform.position;
            EnemyCount++;
        }
        index++;
        yield return new WaitForSeconds(5);
        for (int i = 0; i < 8; i++)
        {
            yield return new WaitForSeconds(0.1f);
            GameObject OBJ = Instantiate(Enemy[0], SummonPos[index], Quaternion.identity);
            OBJ.GetComponent<CharacterManager>().TargetPos = GameManager.Instance.player.Home.transform.position;
            EnemyCount++;
        }
    }
}
