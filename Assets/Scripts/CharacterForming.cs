using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterForming : MonoBehaviour
{
    [SerializeField] Transform[] CharacterStd = new Transform[5];
    [SerializeField] Image[] CharacterIcon = new Image[7];
    GameManager GM;
    Character SelectCharacter;
    int SelectIndex;
    [SerializeField] GameObject Stat;
    public void AddCharacter(int index)
    {
        if (SelectCharacter != null && !Array.Exists(GM.CharacterForm, a => a == SelectCharacter.prefab))
        {
            Transform Tf = CharacterStd[index].Find("CharacterBg");
            Tf.gameObject.SetActive(true);
            Tf.Find("CharacterS").GetComponent<Image>().sprite = SelectCharacter.prefab.GetComponent<Tower>().Stdillust;
            Tf.Find("role").GetComponent<TextMeshProUGUI>().text = SelectCharacter.role;
            Tf.Find("Level").GetComponent<TextMeshProUGUI>().text = "Lv." + (SelectCharacter.Level + 1).ToString();
            Tf.Find("Name").GetComponent<TextMeshProUGUI>().text = SelectCharacter.name;
            GM.CharacterForm[index] = SelectCharacter.prefab;
            CharacterIcon[SelectIndex].transform.parent.Find("Selected").gameObject.SetActive(true);
        }
    }
    public void ViewStat(int index)
    {
        if (GM.CharacterForm[index] != null)
        {
            CharacterManager CM = GM.CharacterForm[index].GetComponent<CharacterManager>();
            Transform Char = Stat.transform;
            Char.Find("Character").Find("CharacterStd").GetComponent<Image>().sprite = CM.StatStdillust;
            Char.Find("Character").Find("CharacterName").GetComponent<TextMeshProUGUI>().text = CM.CharacterName;
            Transform StatText = Char.Find("Stat").Find("StatText");
            //0 체력 / 1 이동속도 / 2 공격력 / 3 공격속도 / 4 장갑 관통 / 5 시야
            List<TextMeshProUGUI> a = new List<TextMeshProUGUI>();
            for (int i = 0; i < StatText.childCount; i++)
            {
                a.Add(StatText.GetChild(i).GetComponent<TextMeshProUGUI>());
            }
            a[0].text = CM.MaxHp.ToString();
            a[1].text = CM.Speed.ToString();
            a[2].text = CM.Atk.ToString();
            a[3].text = CM.AttackSpeed.ToString();
            a[4].text = CM.AntiAmor.ToString() + "%";
            a[5].text = CM.Sight.ToString();
        }
        Stat.SetActive(true);
    }
    public void IconSelect(int index)
    {
        SelectCharacter = null;
        for (int i = 0; i < GM.Characterlist.characters.Length; i++)
        {
            if (GM.Characterlist.characters[i].unlock)
            {
                CharacterIcon[i].color = Color.white;
            }
            else
            {
                CharacterIcon[i].color = new Color(0.4f, 0.4f, 0.4f, 1f);
            }
        }
        if (GM.Characterlist.characters[index].unlock)
        {
            SelectIndex = index;
            CharacterIcon[index].color = new Color(0.7843137f, 0.7843137f, 0.7843137f, 1);
            SelectCharacter = GM.Characterlist.characters[index];
        }
    }
    void OnEnable()
    {
        GM = GameManager.Instance;
        GameObject Icon = transform.Find("IconList").gameObject;
        GameObject Standing = transform.Find("FormedList").gameObject;
        CharacterIcon = new Image[Icon.transform.childCount];
        CharacterStd = new Transform[Standing.transform.childCount];
        for (int i = 0; i < CharacterIcon.Length; i++)
        {
            CharacterIcon[i] = Icon.transform.GetChild(i).Find("Icon").GetComponent<Image>();
            CharacterIcon[i].color = new Color(1, 1, 1, 0);
        }
        for (int i = 0; i < GM.Characterlist.characters.Length; i++)
        {
            CharacterIcon[i].sprite = GM.Characterlist.characters[i].prefab.GetComponent<Tower>().Icon;
            if (GM.Characterlist.characters[i].unlock)
            {
                CharacterIcon[i].color = Color.white;
            }
            else
            {
                CharacterIcon[i].color = new Color(0.4f, 0.4f, 0.4f, 1f);
            }
        }
        for(int i = 0;i<CharacterStd.Length;i++)
        {
            CharacterStd[i] = Standing.transform.GetChild(i);
        }
    }
}
