using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SearchScript : MonoBehaviour
{
    Tower tower;
    GameManager GM;
    void Start()
    {
        tower = GetComponent<Tower>();
        GM = GameManager.Instance;
    }
    public void MakeMine(GameObject OBJ)
    {
        StartCoroutine(minehWait(OBJ));
    }
    IEnumerator minehWait(GameObject mine)
    {
        yield return null;
        float Timer = 0;
        tower.StopCoroutine(tower.MoveCoroutine);
        tower.PlayerOrder = true;
        float a = 0;
        if (mine.GetComponent<Mine>().type == Mine.Type.AntiTank)
            a = 5;
        else
            a = 3;
        GameObject timerText = null;
        if (GM.player.TimerPool.Count > 0)
        {
            timerText = GM.player.TimerPool.Dequeue();
            timerText.transform.position = transform.position;
        }
        else
            timerText = Instantiate(GM.player.Timerprefab, transform.position, Quaternion.identity);
        TextMeshPro text = timerText.GetComponent<TextMeshPro>();
        while (tower.PlayerOrder && Timer < a)
        {
            text.text = (((int)(Timer * 10)) * 0.1f).ToString();
            Timer += Time.deltaTime;
            Debug.Log(Timer);
            yield return null;
        }
        if (Timer >= a)
        {
            GM.player.MineCount++;
            Instantiate(mine, transform.position, Quaternion.identity);
        }
        tower.PlayerOrder = false;
        text.text = "";
        GM.player.TimerPool.Enqueue(timerText);
        yield return null;
    }
}
