using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage : MonoBehaviour
{
    [SerializeField] GameObject[] Enemy;
    Coroutine Coroutine;
    [SerializeField] GameObject BattleEndDialogue;
    GameObject EndTrigger;
    bool SummonEnd;
    void Start()
    {
        Coroutine = StartCoroutine(gameObject.name, 120);
    }
    void Update()
    {
        if (BattleEndDialogue != null && SummonEnd && EndTrigger == null)
        {
            SummonEnd = false;
            BattleEndDialogue.SetActive(true);
            string S = gameObject.name.Replace("Stage", "");
            int StageNum = int.Parse(S);
            if (GameManager.Instance.Stage < StageNum)
                GameManager.Instance.Stage = StageNum;
        }
    }
    IEnumerator Stage1(float Time)
    {
        yield return new WaitForSeconds(Time);
        for (int i = 0; i < 10; i++)
        {
            yield return new WaitForSeconds(0.1f);
            GameObject OBJ = Instantiate(Enemy[0], new Vector2(-20, 7), Quaternion.identity);
            OBJ.GetComponent<CharacterManager>().TargetPos = GameManager.Instance.player.Home.transform.position;
        }
        yield return new WaitForSeconds(20);
        for (int i = 0; i < 8; i++)
        {
            yield return new WaitForSeconds(0.1f);
            GameObject OBJ = Instantiate(Enemy[0], new Vector2(-1, 20), Quaternion.identity);
            OBJ.GetComponent<CharacterManager>().TargetPos = GameManager.Instance.player.Home.transform.position;
            EndTrigger = OBJ;
        }
        SummonEnd = true;
    }
    IEnumerator Stage2(float Time)
    {
        yield return new WaitForSeconds(Time);
        for (int i = 0; i < 10; i++)
        {
            yield return new WaitForSeconds(0.1f);
            GameObject OBJ = Instantiate(Enemy[0], new Vector2(-19, 6), Quaternion.identity);
            OBJ.GetComponent<CharacterManager>().TargetPos = GameManager.Instance.player.Home.transform.position;
        }
        yield return new WaitForSeconds(30);
        for (int i = 0; i < 8; i++)
        {
            yield return new WaitForSeconds(0.1f);
            GameObject OBJ = Instantiate(Enemy[0], new Vector2(-12.5f, 21.5f), Quaternion.identity);
            OBJ.GetComponent<CharacterManager>().TargetPos = GameManager.Instance.player.Home.transform.position;
        }
        yield return new WaitForSeconds(15);
        for (int i = 0; i < 4; i++)
        {
            yield return new WaitForSeconds(0.1f);
            GameObject OBJ = Instantiate(Enemy[0], new Vector2(3, -6), Quaternion.identity);
            OBJ.GetComponent<CharacterManager>().TargetPos = GameManager.Instance.player.Home.transform.position;
            EndTrigger = OBJ;
        }
        SummonEnd = true;
    }
    IEnumerator Stage3(float Time)
    {
        yield return new WaitForSeconds(Time);
        for (int i = 0; i < 10; i++)
        {
            yield return new WaitForSeconds(0.1f);
            GameObject OBJ = Instantiate(Enemy[0], new Vector2(-17, 5.5f), Quaternion.identity);
            OBJ.GetComponent<CharacterManager>().TargetPos = GameManager.Instance.player.Home.transform.position;
        }
        yield return new WaitForSeconds(30);
        for (int i = 0; i < 8; i++)
        {
            yield return new WaitForSeconds(0.1f);
            GameObject OBJ = Instantiate(Enemy[0], new Vector2(-16.5f, -7.5f), Quaternion.identity);
            OBJ.GetComponent<CharacterManager>().TargetPos = GameManager.Instance.player.Home.transform.position;
            EndTrigger = OBJ;
        }
        SummonEnd = true;
    }
}
