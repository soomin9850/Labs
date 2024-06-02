using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class BuildInfo
{
    public Transform Building;
    public int Index;
}
public class MainUI : MonoBehaviour
{
    [SerializeField] Slider[] SoundSlider = new Slider[3];
    [SerializeField] Slider ResolutionSlider;
    [SerializeField] Slider FrameSlider;

    [SerializeField] TextMeshProUGUI RsText;
    [SerializeField] TextMeshProUGUI FrameText;
    [SerializeField] TextMeshProUGUI[] SoundValue = new TextMeshProUGUI[3];
    Vector2 MousePos;
    bool DoubleClick;
    GameManager GM;
    Camera mainCam;
    [SerializeField] GameObject BuildingSet;
    BuildInfo info;
    void Start()
    {
        GM = GameManager.Instance;
        mainCam = Camera.main;
        Sync();
    }
    public void OBJOnOff(GameObject OBJ)
    {
        OBJ.SetActive(!OBJ.activeSelf);
    }
    public void OBJOff(GameObject OBJ)
    {
        OBJ.SetActive(false);
    }
    public void OBJOn(GameObject OBJ)
    {
        OBJ.SetActive(true);
    }
    public void GameExit()
    {
        Application.Quit();
    }

    //게임 중 UI
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
        else if(GM.Character[Index] != null)
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
            if (GM.Character[Index] != null && GM.Character[Index].activeSelf)
            {
                mainCam.transform.position = GM.Character[Index].transform.position;
            }
        }
    }
    public void ListDrag(GameObject Lists)
    {
        float Dis = 0f;
        if (Input.mousePosition.x > MousePos.x)
        {
            Dis = Input.mousePosition.x - MousePos.x;
        }
        else if(Input.mousePosition.x < MousePos.x)
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
        GameObject OBJ = Instantiate(GM.CharacterPrefab[info.Index], info.Building.position, Quaternion.identity);
        GM.Character[info.Index] = OBJ;
        GM.InBuilding(info.Building.GetComponent<CharacterManager>(), OBJ.GetComponent<CharacterManager>());
    }

    //설정
    public void Sync()
    {
        for (int i = 0; i < SoundSlider.Length; i++)
        {
            SoundSlider[i].value = GameManager.Instance.Sound[i];
            SoundValue[i].text = ((int)(GameManager.Instance.Sound[i] * 100)).ToString();
        }
        ResolutionSlider.value = GameManager.Instance.RsIndex;
        RsText.text = GameManager.Instance.Width[(int)ResolutionSlider.value].ToString() + " X " + GameManager.Instance.Height[(int)ResolutionSlider.value].ToString();
        FrameSlider.value = GameManager.Instance.TargetFrameIndex;
        FrameText.text = GameManager.Instance.TargetFrame[GameManager.Instance.TargetFrameIndex].ToString();
    }
    public void ChangeResolution(Slider slider)
    {
        GameManager.Instance.RsIndex = (int)slider.value;
        int setWidth = GameManager.Instance.Width[(int)slider.value];
        int setHeight = GameManager.Instance.Height[(int)slider.value];
        int deviceWidth = Screen.width;
        int deviceHeight = Screen.height;
        Screen.SetResolution(setWidth, (int)(((float)deviceHeight / deviceWidth) * setWidth), true);
        RsText.text = GameManager.Instance.Width[(int)slider.value].ToString() + " X " + GameManager.Instance.Height[(int)slider.value].ToString();
    }
    public void RsPlus(Slider slider)
    {
        slider.value++;
        GameManager.Instance.RsIndex = (int)slider.value;
        ChangeResolution(slider);
    }
    public void RsMinus(Slider slider)
    {
        slider.value--;
        GameManager.Instance.RsIndex = (int)slider.value;
        ChangeResolution(slider);
    }
    public void FrameSet(Slider slider)
    {
        GameManager.Instance.TargetFrameIndex = (int)slider.value;
        Application.targetFrameRate = GameManager.Instance.TargetFrame[(int)slider.value];
        FrameText.text = GameManager.Instance.TargetFrame[(int)slider.value].ToString();
    }
    public void FrameMinus(Slider slider)
    {
        slider.value--;
        GameManager.Instance.TargetFrameIndex = (int)slider.value;
        FrameSet(slider);
    }
    public void FramePlus(Slider slider)
    {
        slider.value++;
        GameManager.Instance.TargetFrameIndex = (int)slider.value;
        FrameSet(slider);
    }
    public void SoundValueSet(int Index)
    {
        GameManager.Instance.Sound[Index] = SoundSlider[Index].value;
        SoundValue[Index].text = ((int)(GameManager.Instance.Sound[Index]*100)).ToString();
    }
    public void SetSoundSlider(int i)
    {
        SoundSlider[i].value = GameManager.Instance.Sound[i];
        SoundValue[i].text = ((int)(GameManager.Instance.Sound[i] * 100)).ToString();
    }
    public void PlusSound(int Index)
    {
        GameManager.Instance.Sound[Index] += 0.01f;
        if (GameManager.Instance.Sound[Index] >= 1)
            GameManager.Instance.Sound[Index] = 1;
        SetSoundSlider(Index);
    }
    public void MinusSound(int Index)
    {
        GameManager.Instance.Sound[Index] -= 0.01f;
        if (GameManager.Instance.Sound[Index] <= 0)
            GameManager.Instance.Sound[Index] = 0;
        SetSoundSlider(Index);
    }

    //코루틴
    IEnumerator CharSpawn(int Index)
    {
        GameObject Icon = GameObject.Find("CharIcon");
        Icon.GetComponent<SpriteRenderer>().sprite = GM.CharacterPrefab[Index].GetComponent<CharacterManager>().Icon;
        while (true)
        {
            Vector2 MousePos = mainCam.ScreenToWorldPoint(Input.mousePosition);
            Icon.transform.position = MousePos;
            if (Input.GetMouseButtonUp(0))
            {
                RaycastHit2D hit = Physics2D.Raycast(MousePos, Vector3.forward, 1, 1 << 9);
                RaycastHit2D hit1 = Physics2D.Raycast(MousePos, Vector3.forward, 1, 1 << 12); // 시야 체크
                if (hit.collider != null && hit1.collider != null)
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
