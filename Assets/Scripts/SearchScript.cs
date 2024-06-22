using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;

public class SearchScript : MonoBehaviour
{
    Tower tower;
    GameManager GM;
    GameObject timerText = null;
    Coroutine mine;
    void Start()
    {
        tower = GetComponent<Tower>();
        GM = GameManager.Instance;
    }
    public void MakeMine(GameObject OBJ)
    {
        Stop();
        mine = StartCoroutine(minehWait(OBJ));
    }
    void StopTimer()
    {
        if (timerText == null)
            return;
        TextMeshPro text = timerText.GetComponent<TextMeshPro>();
        text.text = "";
        GM.player.TimerPool.Enqueue(timerText);
        timerText = null;
    }
    void Stop()
    {
        if (mine != null)
        {
            StopCoroutine(mine);
            mine = null;
        }
        StopTimer();
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
            yield return null;
        }
        if (Timer >= a)
        {
            GM.player.MineCount++;
            Instantiate(mine, transform.position, Quaternion.identity);
        }
        tower.PlayerOrder = false;
        StopTimer();
        mine = null;
        yield return null;
    }
}
