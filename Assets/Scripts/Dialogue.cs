using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Info
{
    public enum LR
    {
        left, right
    }
    public LR lr;
    public Sprite LeftCharacter;
    public Sprite RightCharacter;
    public string Text;

    public int branchCount;
    public string[] ChoiceText;
    public bool Swap;
}
public class Dialogue : MonoBehaviour
{
    [SerializeField] List<Info> dialogues = new List<Info>();

    [SerializeField] Image[] Standing = new Image[2];
    [SerializeField] TextMeshProUGUI DialogueText;
    [SerializeField] GameObject DialogueOBJ;
    [SerializeField] TextMeshProUGUI[] branchButtonText = new TextMeshProUGUI[4];
    [SerializeField] GameObject Branch;
    WaitForSeconds WaitForSeconds = new WaitForSeconds(0.1f);
    int ChoiceNum;
    void OnEnable()
    {
        ChoiceNum = 0;
        Time.timeScale = 0;
        StartCoroutine(StartDialLogue());
    }
    IEnumerator StartDialLogue()
    {
        int i = 0;
        DialogueOBJ.SetActive(true);
        while (dialogues.Count > i)
        {
            ChoiceNum = 0;
            
            int LR = (int)dialogues[i].lr;
            string Text = dialogues[i].Text.Replace("\\n", "\n");
            if (LR == 0)
            {
                Standing[0].color = Color.white;
                Standing[1].color = new Color(1, 1, 1, 0.5f);
            }
            else
            {
                Standing[1].color = Color.white;
                Standing[0].color = new Color(1, 1, 1, 0.5f);
            }
            Standing[0].sprite = dialogues[i].LeftCharacter;
            Standing[1].sprite = dialogues[i].RightCharacter;

            if (dialogues[i].RightCharacter == null)
                Standing[1].color = new Color(0, 0, 0, 0);
            if (dialogues[i].LeftCharacter == null)
                Standing[0].color = new Color(0, 0, 0, 0);

            DialogueText.text = Text;
            yield return null;
            yield return new WaitUntil(() => Input.GetMouseButtonDown(0));
            if (dialogues[i].branchCount > 0)
            {
                Branch.SetActive(true);
                for (int j = 0; j < dialogues[i].branchCount; j++)
                {
                    branchButtonText[j].transform.parent.gameObject.SetActive(true);
                    string BText = dialogues[i].ChoiceText[j].Replace("\\n", "\n");
                    branchButtonText[j].text = BText;
                }
                yield return new WaitUntil(() => ChoiceNum != 0);
                if (dialogues[i].Swap)
                    SwapChoice(i);
                Branch.SetActive(false);
                for (int j = 0; j < branchButtonText.Length; j++)
                {
                    branchButtonText[j].transform.parent.gameObject.SetActive(false);
                }
            }
            i++;
        }
        DialogueOBJ.SetActive(false);
        Time.timeScale = 1;
        yield return null;
    }
    void SwapChoice(int now)
    {
        if (ChoiceNum == 2)
        {
            Info info = dialogues[now + 1];
            dialogues[now + 1] = dialogues[now + 2];
            dialogues[now + 2] = info;
        }
    }
    public void Choice(int Index)
    {
        ChoiceNum = Index;
    }
}