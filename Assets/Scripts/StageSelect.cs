using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StageSelect : MonoBehaviour
{
    string StageName = "Stage1";
    [SerializeField] GameObject[] StageIcon = new GameObject[7];
    [SerializeField] TextMeshProUGUI text;
    [SerializeField] TextMeshProUGUI StageInfo;
    [SerializeField] Image StageImage;
    Vector2 MousePos;
    void OnEnable()
    {
        for(int i = 0; i < StageIcon.Length; i++)
        {
            StageIcon[i].transform.parent.GetComponent<Button>().interactable = true;
        }
        for (int i = GameManager.Instance.Stage + 1; i < StageIcon.Length; i++)
        {
            StageIcon[i].transform.parent.GetComponent<Button>().interactable = false;
        }
    }
    public void Stage(string name)
    {
        StageName = name;
        text.text = StageName;
    }
    public void Focus(int Stage)
    {
        for (int i = 0; i < StageIcon.Length; i++)
        {
            if (i == Stage)
            {
                StageIcon[i].SetActive(true);
                continue;
            }
            StageIcon[i].SetActive(false);
        }
    }
    public void Info(string info)
    {
        StageInfo.text = info.Replace("\\n", "\n");
    }
    public void StageEnter()
    {
        int Count = 0;
        for (int i = 0; i < GameManager.Instance.CharacterForm.Length; i++)
        {
            if (GameManager.Instance.CharacterForm[i] != null)
                Count++;
        }
        if (Count > 0)
            SceneManager.LoadScene(StageName);
    }
    public void ClickMoustPos()
    {
        MousePos = Input.mousePosition;
    }
    public void StageDrag(GameObject Stage)
    {
        float Dis = 0f;
        if (Input.mousePosition.x > MousePos.x)
        {
            Dis = Input.mousePosition.x - MousePos.x;
        }
        else if (Input.mousePosition.x < MousePos.x)
        {
            Dis = Input.mousePosition.x - MousePos.x;
        }
        RectTransform rectT = Stage.GetComponent<RectTransform>();
        rectT.anchoredPosition += new Vector2(Dis, 0);

        if (rectT.anchoredPosition.x < -900f) rectT.anchoredPosition = new Vector2(-900f, -115f);
        else if (rectT.anchoredPosition.x > -260f) rectT.anchoredPosition = new Vector2(-260f, -115f);

        MousePos = Input.mousePosition;
    }
}