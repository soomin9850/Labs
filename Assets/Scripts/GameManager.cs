using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public Player player;
    public Queue<GameObject> bulletZero = new Queue<GameObject>();
    public Queue<GameObject> bulletOne = new Queue<GameObject>();
    public Tilemap WallTile;
    public Tilemap PathFindTile;

    public CharacterInfo Characterlist;
    public GameObject[] Character = new GameObject[5];
    public GameObject[] CharacterForm = new GameObject[5];
    public TileBase TileBase;

    public float[] Sound = { 1, 1, 1 }; //0 : 전체, 1 : 배경, 2 : 캐릭터

    public int[] Width;
    public int[] Height;
    public int RsIndex = 0;

    public int[] TargetFrame;
    public int TargetFrameIndex = 1;

    public float[] BuildingSight;

    public int Stage = 0;

    public Dictionary<string, KeyCode> Keys = new Dictionary<string, KeyCode>();
    void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
        SceneManager.LoadScene("Main");
    }
    public void InBuilding(CharacterManager Building, CharacterManager Character)
    {
        Building B = Building.GetComponent<Building>();
        B.InPlayer = true;
        Building.type = Character.type;
        if (Character.type == CharacterManager.Type.machineGun)
        {
            Building.MGStat = Character.MGStat;
            Building.attackDel = Building.MgAttack;
            Building.attackEnd = Building.MgAttackEnd;
            Building.MGStat.ReLoadTime = Character.MGStat.ReLoadTime;
            Building.MGStat.ShootTime = Character.MGStat.ShootTime;
            Building.MGStat.Timer = Character.MGStat.Timer;
        }
        Building.MaxHp = Character.MaxHp;
        Building.HP = Character.HP;
        Building.Icon = Character.Icon;
        Building.Sight = Character.Sight * BuildingSight[B.BuildingSize];
        Building.Speed = Character.Speed;
        Building.AttackSpeed = Character.AttackSpeed;
        Building.Atk = Character.Atk;
        Building.Bullet = Character.Bullet;
        Building.transform.tag = "Player";
        Building.AntiAmor = Character.AntiAmor;
        Transform Child = Building.transform.GetChild(Building.transform.childCount - 1);
        float S = Building.Sight * 2f;
        Child.parent = null;
        Child.localScale = new Vector3(S, S, S);
        Child.parent = Building.transform;
        Building.transform.GetChild(2).GetComponent<SpriteRenderer>().sprite = Character.Icon;
        Child.gameObject.SetActive(true);
        Character.transform.gameObject.transform.parent = Building.transform.gameObject.transform;
        Building.transform.GetChild(0).gameObject.SetActive(true);
        StartCoroutine(Wait(Character.transform.gameObject));
    }
    IEnumerator Wait(GameObject OBJ)
    {
        yield return null;
        OBJ.SetActive(false);
    }
    public void OutBuilding(CharacterManager Building, CharacterManager Character)
    {
        Building B = Building.GetComponent<Building>();
        GameObject Char = Character.transform.gameObject;
        Building.transform.GetChild(2).GetComponent<SpriteRenderer>().sprite = null;
        Building.transform.GetChild(0).gameObject.SetActive(false);
        Character.HP = Building.HP;
        Char.transform.parent = null;
        Char.transform.position += B.ExitDir;
        B.InPlayer = false;
        Building.transform.tag = "Building";
        Transform Child = Building.transform.GetChild(Building.transform.childCount - 1);
        Child.gameObject.SetActive(false);
        Char.SetActive(true);
        Building.Target = null;
    }
}