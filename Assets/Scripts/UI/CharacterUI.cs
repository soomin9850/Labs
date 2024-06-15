using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterUI : MonoBehaviour
{
    Vector2 MousePos;
    bool DoubleClick;
    GameManager GM;
    Camera mainCam;
    [SerializeField] GameObject BuildingSet;
    BuildInfo info;
    [SerializeField] GameObject Stat;
    [SerializeField] GameObject Icon;
    void Awake()
    {
        Icon = GameObject.Find("CharIcon");
        GM = GameManager.Instance;
        mainCam = Camera.main;
        for(int i=0;i<GM.CharacterForm.Length;i++)
        {
            Image image = GameObject.Find("List" + i.ToString()).transform.GetChild(0).GetComponent<Image>();
            image.color = new Color(1, 1, 1, 0);
            if (GM.CharacterForm[i] != null)
            {
                image.color = new Color(1, 1, 1, 1);
                image.sprite = GM.CharacterForm[i].GetComponent<CharacterManager>().Icon;
            }
        }
    }
    public void CharList(GameObject List)
    {
        RectTransform rectT = List.GetComponent<RectTransform>();

        if (rectT.anchoredPosition.x < 0) rectT.anchoredPosition = new Vector2(0, 0);
        else if (rectT.anchoredPosition.x == 0) rectT.anchoredPosition = new Vector2(-500, 0);
    }
    public void ClickMoustPos()
    {
        MousePos = Input.mousePosition;
    }
    public void CharClick(int Index)
    {
        if (GM.Character[Index] == null)
        {
            StartCoroutine(CharSpawn(Index));
            return;
        }
        else if (GM.Character[Index] != null)
        {
            mainCam.transform.position = GM.Character[Index].transform.position;
        }
        if (!DoubleClick)
        {
            DoubleClick = true;
            StartCoroutine(doubleClick());
        }
        else if (DoubleClick)
        {
            Debug.Log("DoubleClick");
            if (GM.Character[Index] != null)
            {
                CharacterManager CM = GM.Character[Index].GetComponent<CharacterManager>();
                Transform Char = Stat.transform;
                Char.Find("Character").Find("CharacterStd").GetComponent<Image>().sprite = CM.StatStdillust;
                Char.Find("Character").Find("CharacterName").GetComponent<TextMeshProUGUI>().text = CM.CharacterName;
                Transform StatText = Char.Find("Stat").Find("StatText");
                //0 체력 / 1 이동속도 / 2 공격력 / 3 공격속도 / 4 장갑 관통 / 5 시야
                List<TextMeshProUGUI> a = new List<TextMeshProUGUI>();
                for(int i = 0; i < StatText.childCount; i++)
                {
                    a.Add(StatText.GetChild(i).GetComponent<TextMeshProUGUI>());
                }
                a[0].text = CM.HP.ToString();
                a[1].text = CM.Speed.ToString();
                a[2].text = CM.Atk.ToString();
                a[3].text = CM.AttackSpeed.ToString();
                a[4].text = CM.AntiAmor.ToString() + "%";
                a[5].text = CM.Sight.ToString();
            }
            Stat.SetActive(true);
        }
    }
    public void ListDrag(GameObject Lists)
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
        RectTransform rectT = Lists.GetComponent<RectTransform>();
        rectT.anchoredPosition += new Vector2(Dis, 0);

        if (rectT.anchoredPosition.x < -1400f) rectT.anchoredPosition = new Vector2(-1400, 0);
        else if (rectT.anchoredPosition.x > 0) rectT.anchoredPosition = new Vector2(0, 0);

        MousePos = Input.mousePosition;
    }
    public void SpawnCharacter()
    {
        GameObject OBJ = Instantiate(GM.CharacterForm[info.Index], info.Building.position, Quaternion.identity);
        GM.Character[info.Index] = OBJ;
        GM.InBuilding(info.Building.GetComponent<CharacterManager>(), OBJ.GetComponent<CharacterManager>());
        GM.player.LoseTrigger.Add(OBJ);
    }
    IEnumerator CharSpawn(int Index)
    {
        Icon.GetComponent<SpriteRenderer>().sprite = GM.CharacterForm[Index].GetComponent<CharacterManager>().Icon;
        while (true)
        {
            Vector2 MousePos = mainCam.ScreenToWorldPoint(Input.mousePosition);
            Icon.transform.position = MousePos;
            if (Input.GetMouseButtonUp(0))
            {
                RaycastHit2D hit = Physics2D.Raycast(MousePos, Vector3.forward, 1, 1 << 9);
                RaycastHit2D hit1 = Physics2D.Raycast(MousePos, Vector3.forward, 1, 1 << 12); // 시야 체크
                if (hit.collider != null && hit1.collider != null && !hit.transform.GetComponent<Building>().InPlayer)
                {
                    BuildingSet.SetActive(true);
                    info = new BuildInfo();
                    info.Building = hit.transform;
                    info.Index = Index;
                }
                break;
            }
            yield return null;
        }
        Icon.GetComponent<SpriteRenderer>().sprite = null;
    }
    IEnumerator doubleClick()
    {
        yield return new WaitForSeconds(0.2f);
        DoubleClick = false;
    }
}
