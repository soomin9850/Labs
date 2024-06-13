using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
public class Player : MonoBehaviour
{
    [SerializeField] Camera mainCam;
    Vector2 BeforePos;
    Vector2 AfterPos;
    Vector2 Center;
    Vector2 Size;
    [SerializeField] GameObject Drag;
    [SerializeField] GameObject Setting;
    [SerializeField] GameObject Shadow;
    public List<GameObject> SelectCharacter = new List<GameObject>();
    public List<GameObject> SelectBuilding = new List<GameObject>();

    public GameObject Timerprefab;
    public Queue<GameObject> TimerPool = new Queue<GameObject>();

    public GameObject Home;

    public int MineCount;
    public int MineMaxCount;
    public GameObject TankMine;
    public GameObject HumanMine;

    public int TrenchMaxCount;
    public int TrenchCount;
    public GameObject Trench;

    public int BombCount;
    public int BombMaxCount;
    public GameObject Bomb;

    SpriteRenderer BombRenderer;

    GameManager GM;

    public Stage Stage;

    public List<GameObject> LoseTrigger;
    void Awake()
    {
        GM = GameManager.Instance;
        mainCam = Camera.main;
        GM.WallTile = GameObject.Find("Wall").GetComponent<Tilemap>();
        GM.PathFindTile = GameObject.Find("PathFindingWall").GetComponent<Tilemap>();
        //GM.BuildingTile.color = new Color(0, 0, 0, 0);
        GM.player = this;
        Shadow.GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, 1);
    }
    void Start()
    {
        GameObject Temp = null;
        int TempIndex = 0;
        for (int i = 0; i < GM.CharacterForm.Length; i++)
        {
            if (GM.CharacterForm[i] == null)
                continue;
            if (GM.CharacterForm[i].GetComponent<CharacterManager>().CharacterName == "Sniper")
            {
                TempIndex = i;
                Temp = GM.CharacterForm[i];
            }
        }
        GameObject OBJ = Instantiate(Temp, Home.transform.position, Quaternion.identity);
        GM.Character[TempIndex] = OBJ;
        GM.InBuilding(Home.transform.GetComponent<CharacterManager>(), OBJ.GetComponent<CharacterManager>());
        LoseTrigger.Add(OBJ);
    }
    void Update()
    {
        CameraMove();
        SelectMouse();
        MoveMouse();
        BuildingEnteraction();
        Enteraction();
        BombClick();
        GameLose();
    }
    void GameLose()
    {
        if (LoseTrigger.Count <= 0 || !Home.TryGetComponent(out Building building))
        {
            SceneManager.LoadScene("Main");
        }
    }
    void BombClick()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit2D hit = Physics2D.Raycast((Vector2)mainCam.ScreenToWorldPoint(Input.mousePosition), transform.forward, 5, 1 << 15);
            if (hit.collider != null)
            {
                BombRenderer = hit.transform.GetComponent<SpriteRenderer>();
                BombRenderer.color = Color.red - new Color(0, 0, 0, 0.5f);
            }
        }
        if (Input.GetMouseButtonUp(0))
        {
            if(BombRenderer != null)
                BombRenderer.color = Color.red;
            RaycastHit2D hit = Physics2D.Raycast((Vector2)mainCam.ScreenToWorldPoint(Input.mousePosition), transform.forward, 5, 1 << 15);
            if(hit.collider != null)
            {
                hit.transform.GetComponent<Bomb>().Click();
            }
        }
    }
    void Enteraction()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            for (int i = 0; i < SelectCharacter.Count; i++)
            {
                CharacterManager CM = SelectCharacter[i].GetComponent<CharacterManager>();
                if (CM.type == CharacterManager.Type.engineer || CM.type == CharacterManager.Type.rifle)
                {
                    RaycastHit2D[] hits = Physics2D.CircleCastAll(SelectCharacter[i].transform.position, 1, transform.forward, 1, 1 << 0);
                    for (int j = 0; j < hits.Length; j++)
                    {
                        if (hits[j].transform.CompareTag("Car"))
                        {
                            float XDis = hits[j].transform.position.x - SelectCharacter[i].transform.position.x;
                            float YDis = hits[j].transform.position.y - SelectCharacter[i].transform.position.y;
                            if (MathF.Abs(XDis) > MathF.Abs(YDis))
                            {
                                if (XDis < 0)
                                    hits[j].transform.GetComponent<Car>().Move(Vector2.left);
                                else if (XDis > 0)
                                    hits[j].transform.GetComponent<Car>().Move(Vector2.right);
                            }
                            else
                            {
                                if (YDis < 0)
                                    hits[j].transform.GetComponent<Car>().Move(Vector2.down);
                                else if (YDis > 0)
                                    hits[j].transform.GetComponent<Car>().Move(Vector2.up);
                            }
                            return;
                        }
                    }
                }
            }
        }
        else if (Input.GetKey(KeyCode.X) && Input.GetMouseButtonDown(1) && BombCount < BombMaxCount) //°Ç¹°ÆøÆÄ
        {
            for (int i = 0; i < SelectCharacter.Count; i++)
            {
                if (SelectCharacter[i].GetComponent<CharacterManager>().type == CharacterManager.Type.engineer)
                {
                    RaycastHit2D hit = Physics2D.Raycast((Vector2)mainCam.ScreenToWorldPoint(Input.mousePosition), transform.forward, 5, 1 << 9);
                    RaycastHit2D hit1 = Physics2D.Raycast((Vector2)mainCam.ScreenToWorldPoint(Input.mousePosition), transform.forward, 5, 1 << 0);
                    if (hit1.collider != null && hit1.transform.CompareTag("Bridge"))
                    {
                        SelectCharacter[i].GetComponent<EngineerScript>().MakeBomb(hit1.transform.gameObject);
                        return;
                    }
                    if (hit.collider != null)
                    {
                        SelectCharacter[i].GetComponent<EngineerScript>().MakeBomb(hit.transform.gameObject);
                        return;
                    }
                }
            }
        }
        else if (Input.GetKeyDown(KeyCode.Z) && TrenchCount < TrenchMaxCount) // ÂüÈ£
        {
            for (int i = 0; i < SelectCharacter.Count; i++)
            {
                if (SelectCharacter[i].GetComponent<CharacterManager>().type == CharacterManager.Type.engineer)
                {
                    SelectCharacter[i].GetComponent<EngineerScript>().MakeTrench();
                    return;
                }
            }
        }
        else if(Input.GetKeyDown(KeyCode.C) && MineCount < MineMaxCount) //´ëÀÎ¿ëÁö·Ú
        {
            for (int i = 0; i < SelectCharacter.Count; i++)
            {
                if (SelectCharacter[i].GetComponent<CharacterManager>().type == CharacterManager.Type.search)
                {
                    SelectCharacter[i].GetComponent<SearchScript>().MakeMine(HumanMine);
                }
            }
        }
        else if (Input.GetKeyDown(KeyCode.V) && MineCount < MineMaxCount) //´ëÀüÂ÷Áö·Ú
        {
            for (int i = 0; i < SelectCharacter.Count; i++)
            {
                if (SelectCharacter[i].GetComponent<CharacterManager>().type == CharacterManager.Type.search)
                {
                    SelectCharacter[i].GetComponent<SearchScript>().MakeMine(TankMine);
                }
            }
        }
    }
    void BuildingEnteraction()
    {
        if (Input.GetKeyDown(KeyCode.O))
        {
            for (int i = 0; i < SelectBuilding.Count; i++)
            {
                Building CM = SelectBuilding[i].GetComponent<Building>();
                if (CM.InPlayer)
                    GM.OutBuilding(CM, CM.transform.GetChild(CM.transform.childCount - 1).transform.GetComponent<CharacterManager>());
            }
        }
        else if (Input.GetKey(KeyCode.I) && Input.GetMouseButtonDown(1))
        {
            RaycastHit2D hit = Physics2D.Raycast(mainCam.ScreenToWorldPoint(Input.mousePosition), transform.forward, 2, 1 << 9);
            if (hit.collider != null)
            {
                for(int i = 0;i < SelectCharacter.Count;i++)
                {
                    Tower tower = SelectCharacter[i].GetComponent<Tower>();
                    tower.StartCoroutine(tower.GoInBuilding(hit.transform.gameObject));
                }
            }
        }
    }
    void CameraMove()
    {
        float scroll= Input.GetAxis("Mouse ScrollWheel");
        float X = Input.GetAxis("Horizontal");
        float Y = Input.GetAxis("Vertical");
        mainCam.transform.position += new Vector3(X * Time.deltaTime * 5, Y * Time.deltaTime * 5, 0);
        mainCam.orthographicSize -= scroll * 2;
    }
    void MoveMouse()
    {
        if(Input.GetMouseButtonDown(1))
        {
            RaycastHit2D hit = Physics2D.Raycast(mainCam.ScreenToWorldPoint(Input.mousePosition), transform.forward, 2, 1 << 7);
            if(hit.collider != null)
            {
                for (int i = 0; i < SelectCharacter.Count; i++)
                {
                    if (!SelectCharacter[i].activeSelf)
                        continue;
                    Tower tower = SelectCharacter[i].GetComponent<Tower>();
                    tower.TargetPos = mainCam.ScreenToWorldPoint(Input.mousePosition);
                    tower.Target = hit.transform.gameObject;
                    tower.StopCoroutine(tower.AttackCoroutine);
                    tower.StopCoroutine(tower.MoveCoroutine);
                    tower.ThatAttack(tower.Target);
                }
                for (int i = 0; i < SelectBuilding.Count; i++)
                {
                    if (!SelectBuilding[i].activeSelf)
                        continue;
                    Building building = SelectBuilding[i].GetComponent<Building>();
                    building.Target = hit.transform.gameObject;
                    building.StopCoroutine(building.AttackCoroutine);
                    building.ThatAttack(building.Target);
                }
                return;
            }
            for (int i = 0; i < SelectCharacter.Count; i++)
            {
                if (!SelectCharacter[i].activeSelf)
                    continue;
                CharacterManager tower = SelectCharacter[i].GetComponent<Tower>();
                tower.StopCoroutine(tower.AttackCoroutine);
                tower.StopCoroutine(tower.MoveCoroutine);
                tower.TargetPos = mainCam.ScreenToWorldPoint(Input.mousePosition);
                tower.Target = null;
                tower.AIStart();
            }
        }
    }
    void SelectMouse()
    {
        if (EventSystem.current.IsPointerOverGameObject()) return;
        if (Input.GetMouseButtonDown(0))
        {
            BeforePos = mainCam.ScreenToWorldPoint(Input.mousePosition);
            Drag.SetActive(true);
        }
        if (Input.GetMouseButton(0))
        {
            AfterPos = mainCam.ScreenToWorldPoint(Input.mousePosition);
            float X = Mathf.Abs(BeforePos.x - AfterPos.x);
            float Y = Mathf.Abs(BeforePos.y - AfterPos.y);
            Center = BeforePos + (AfterPos - BeforePos) / 2;
            Size = new Vector2(X, Y);
            Drag.transform.position = Center;
            Drag.transform.localScale = Size;
        }
        if (Input.GetMouseButtonUp(0))
        {
            CharacterSelect(Center, Size);
            Drag.SetActive(false);
        }
    }
    void CharacterSelect(Vector2 Center,Vector2 Size)
    {
        RaycastHit2D[] hits = Physics2D.BoxCastAll(Center, Size, 0, transform.forward, 1, 1 << 10);
        for (int i = 0; i < SelectCharacter.Count; i++)
        {
            SelectCharacter[i].transform.GetComponent<SpriteRenderer>().color = Color.white;
        }
        SelectCharacter.Clear();
        SelectBuilding.Clear();
        if (hits.Length > 0)
        {
            for(int i = 0; i < hits.Length; i++)
            {
                if (hits[i].transform.CompareTag("Player"))
                {
                    if (hits[i].transform.TryGetComponent(out Building building))
                    {
                        SelectBuilding.Add(hits[i].transform.gameObject);
                    }
                    else
                    {
                        SelectCharacter.Add(hits[i].transform.gameObject);
                        hits[i].transform.GetComponent<SpriteRenderer>().color = Color.yellow;
                    }
                }
            }
        }
    }
}
