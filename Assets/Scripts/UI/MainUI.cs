using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static CharacterManager;
using static System.Runtime.CompilerServices.RuntimeHelpers;
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
    [SerializeField] GameObject KeySettingWaitShadow;
    [SerializeField] GameObject KeysUI;
    Dictionary<string, TextMeshProUGUI> ControllKeysText = new Dictionary<string, TextMeshProUGUI>();

    GameManager GM;
    void Start()
    {
        GM = GameManager.Instance;
        int Re = 0;
        foreach (KeyValuePair<string, KeyCode> kv in GM.Keys)
        {
            ControllKeysText[kv.Key] = KeysUI.transform.GetChild(Re).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>();
            Re++;
        }
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
    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
    public void Save()
    {
        GM.Save();
    }
    public void KeySetting(string key)
    {
        KeySettingWaitShadow.SetActive(true);
        StartCoroutine(KeyPressWait(key));
    }
    IEnumerator KeyPressWait(string key)
    {
        while (true)
        {
            if (Input.anyKeyDown)
            {
                foreach (KeyCode code in Enum.GetValues(typeof(KeyCode)))
                {
                    if (Input.GetKeyDown(code))
                    {
                        if (code == KeyCode.Mouse0 || code == KeyCode.Mouse1 || code == KeyCode.W || code == KeyCode.A || code == KeyCode.S || code == KeyCode.D || GM.Keys.ContainsValue(code))
                            continue;
                        GM.Keys[key] = code;
                        KeySettingWaitShadow.SetActive(false);
                        ControllKeysText[key].text = GM.Keys[key].ToString();
                        yield break;
                    }
                        
                }
            }
            yield return null;
        }
    }
    //¼³Á¤
    public void Sync()
    {
        foreach (KeyValuePair<string, KeyCode> kv in GM.Keys)
        {
            ControllKeysText[kv.Key].text = kv.Value.ToString();
        }
        for (int i = 0; i < SoundSlider.Length; i++)
        {
            SoundSlider[i].value = GM.Sound[i];
            SoundValue[i].text = ((int)(GM.Sound[i] * 100)).ToString();
        }
        ResolutionSlider.value = GM.RsIndex;
        RsText.text = GM.Width[(int)ResolutionSlider.value].ToString() + " X " + GM.Height[(int)ResolutionSlider.value].ToString();
        FrameSlider.value = GM.TargetFrameIndex;
        FrameText.text = GM.TargetFrame[GM.TargetFrameIndex].ToString();
    }
    public void ChangeResolution(Slider slider)
    {
        int Value = (int)slider.value;
        GM.RsIndex = Value;
        int setWidth = GM.Width[Value];
        int setHeight = GM.Height[Value];
        int deviceWidth = Screen.width;
        int deviceHeight = Screen.height;
        Screen.SetResolution(setWidth, (int)(((float)deviceHeight / deviceWidth) * setWidth), true);
        RsText.text = GM.Width[Value].ToString() + " X " + GM.Height[(int)slider.value].ToString();
    }
    public void RsPlus(Slider slider)
    {
        slider.value++;
        GM.RsIndex = (int)slider.value;
        ChangeResolution(slider);
    }
    public void RsMinus(Slider slider)
    {
        slider.value--;
        GM.RsIndex = (int)slider.value;
        ChangeResolution(slider);
    }
    public void FrameSet(Slider slider)
    {
        int Value = (int)slider.value;
        GM.TargetFrameIndex = Value;
        Application.targetFrameRate = GM.TargetFrame[Value];
        FrameText.text = GM.TargetFrame[Value].ToString();
    }
    public void FrameMinus(Slider slider)
    {
        slider.value--;
        GM.TargetFrameIndex = (int)slider.value;
        FrameSet(slider);
    }
    public void FramePlus(Slider slider)
    {
        slider.value++;
        GM.TargetFrameIndex = (int)slider.value;
        FrameSet(slider);
    }
    public void SoundValueSet(int Index)
    {
        GM.Sound[Index] = SoundSlider[Index].value;
        SoundValue[Index].text = ((int)(GM.Sound[Index]*100)).ToString();
    }
    public void SetSoundSlider(int i)
    {
        SoundSlider[i].value = GM.Sound[i];
        SoundValue[i].text = ((int)(GM.Sound[i] * 100)).ToString();
    }
    public void PlusSound(int Index)
    {
        GM.Sound[Index] += 0.01f;
        if (GM.Sound[Index] >= 1)
            GM.Sound[Index] = 1;
        SetSoundSlider(Index);
    }
    public void MinusSound(int Index)
    {
        GM.Sound[Index] -= 0.01f;
        if (GM.Sound[Index] <= 0)
            GM.Sound[Index] = 0;
        SetSoundSlider(Index);
    }
}
