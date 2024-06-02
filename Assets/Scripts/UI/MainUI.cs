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
    void Start()
    {
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
    //¼³Á¤
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
}
