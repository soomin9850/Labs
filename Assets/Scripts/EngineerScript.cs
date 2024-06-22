using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EngineerScript : MonoBehaviour
{
    Tower tower;
    GameManager GM;
    Coroutine Fix;
    Coroutine Make;
    Coroutine Trench;
    GameObject timerText = null;
    void Start()
    {
        tower = GetComponent<Tower>();
        GM = GameManager.Instance;
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
        if (Make != null)
        {
            StopCoroutine(Make);
            Make = null;
        }
        else if(Fix != null)
        {
            StopCoroutine(Fix);
            Fix = null;
        }
        else if (Trench != null)
        {
            StopCoroutine(Trench);
            Trench = null;
        }
        StopTimer();
    }
    public void MakeBomb(GameObject OBJ)
    {
        if (OBJ.TryGetComponent(out Building building))
        {
            Stop();
            Make=StartCoroutine(BuildWait(OBJ, 10 + (building.BuildingSize * 5)));
        }
        else if (OBJ.CompareTag("Bridge"))
        {
            Stop();
            if (OBJ.name.Contains("Big"))
            {
                Make=StartCoroutine(BuildWait(OBJ, 20));
            }
            else if (OBJ.name.Contains("Midium"))
            {
                Make=StartCoroutine(BuildWait(OBJ, 15));
            }
            else if (OBJ.name.Contains("Small"))
            {
                Make=StartCoroutine(BuildWait(OBJ, 10));
            }
        }
    }
    public void TrenchFix(GameObject Target)
    {
        Stop();
        Fix = StartCoroutine(TrenchFIxCo(Target));
    }
    public void MakeTrench()
    {
        Stop();
        Trench = StartCoroutine(TrenchWait());
    }
    public void SummonBomb(GameObject Target)
    {
        GM.player.BombCount++;
        GameObject Bomb = Instantiate(GM.player.Bomb, Target.transform.position, Quaternion.identity);
        Bomb.GetComponent<Bomb>().Target = Target;
    }
    IEnumerator TrenchFIxCo(GameObject OBJ)
    {
        Building building = OBJ.GetComponent<Building>();
        yield return null;
        while (Vector2.Distance(OBJ.transform.position, transform.position) > 2f)
        {
            yield return null;
        }
        float Timer = 0;
        tower.StopCoroutine(tower.MoveCoroutine);
        tower.PlayerOrder = true;
        while (tower.PlayerOrder)
        {
            Timer += Time.deltaTime;
            if (Timer >= 1)
            {
                Timer = 0;
                building.Barrier += 10;
            }
            yield return null;
        }
        tower.PlayerOrder = false;
        yield return null;
        Fix = null;
    }
    IEnumerator BuildWait(GameObject OBJ,float a)
    {
        yield return null;
        while (Vector2.Distance(OBJ.transform.position, transform.position) > 3f)
        {
            yield return null;
        }
        float Timer = 0;
        tower.StopCoroutine(tower.MoveCoroutine);
        tower.PlayerOrder = true;
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
            SummonBomb(OBJ);
        tower.PlayerOrder = false;
        StopTimer();
        yield return null;
        Make = null;
    }
    IEnumerator TrenchWait()
    {
        yield return null;
        float Timer = 0;
        tower.StopCoroutine(tower.MoveCoroutine);
        tower.PlayerOrder = true;
        if (GM.player.TimerPool.Count > 0)
        {
            timerText = GM.player.TimerPool.Dequeue();
            timerText.transform.position = transform.position;
        }
        else
            timerText = Instantiate(GM.player.Timerprefab, transform.position, Quaternion.identity);
        TextMeshPro text = timerText.GetComponent<TextMeshPro>();
        while (tower.PlayerOrder && Timer < 15)
        {
            text.text = (((int)(Timer * 10)) * 0.1f).ToString();
            Timer += Time.deltaTime;
            yield return null;
        }
        if (Timer >= 15)
        {
            GM.player.TrenchCount++;
            Vector2 Temp = new Vector2(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y));
            Instantiate(GM.player.Trench, Temp + new Vector2(0.5f, 0.5f), Quaternion.identity);
        }
        tower.PlayerOrder = false;
        StopTimer();
        yield return null;
        Trench = null;
    }
}
