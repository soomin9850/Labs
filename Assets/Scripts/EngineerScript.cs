using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EngineerScript : MonoBehaviour
{
    Tower tower;
    GameManager GM;
    void Start()
    {
        tower = GetComponent<Tower>();
        GM = GameManager.Instance;
    }
    public void MakeBomb(GameObject OBJ)
    {
        if (OBJ.TryGetComponent(out Building building))
        {
            StartCoroutine(BuildWait(OBJ, 10 + (building.BuildingSize * 5)));
        }
        else if (OBJ.CompareTag("Bridge"))
        {
            if (OBJ.name.Contains("Big"))
            {
                StartCoroutine(BuildWait(OBJ, 20));
            }
            else if (OBJ.name.Contains("Midium"))
            {
                StartCoroutine(BuildWait(OBJ, 15));
            }
            else if (OBJ.name.Contains("Small"))
            {
                StartCoroutine(BuildWait(OBJ, 10));
            }
        }
    }
    public void TrenchFix()
    {

    }
    public void MakeTrench()
    {
        StartCoroutine(TrenchWait());
    }
    public void SummonBomb(GameObject Target)
    {
        GM.player.BombCount++;
        GameObject Bomb = Instantiate(GM.player.Bomb, Target.transform.position, Quaternion.identity);
        Bomb.GetComponent<Bomb>().Target = Target;
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
            SummonBomb(OBJ);
        tower.PlayerOrder = false;
        tower.PlayerOrder = false;
        text.text = "";
        GM.player.TimerPool.Enqueue(timerText);
        yield return null;
    }
    IEnumerator TrenchWait()
    {
        yield return null;
        float Timer = 0;
        tower.StopCoroutine(tower.MoveCoroutine);
        tower.PlayerOrder = true;
        GameObject timerText = null;
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
            Debug.Log(Timer);
            yield return null;
        }
        if (Timer >= 15)
        {
            GM.player.TrenchCount++;
            Vector2 Temp = new Vector2(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y));
            Instantiate(GM.player.Trench, Temp + new Vector2(0.5f, 0.5f), Quaternion.identity);
        }
        tower.PlayerOrder = false;
        text.text = "";
        GM.player.TimerPool.Enqueue(timerText);
        yield return null;
    }
}
