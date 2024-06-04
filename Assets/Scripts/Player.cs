using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
public class Player : MonoBehaviour
{
    Camera mainCam;
    Vector2 BeforePos;
    Vector2 AfterPos;
    Vector2 Center;
    Vector2 Size;
    [SerializeField] GameObject Drag;
    [SerializeField] GameObject Setting;
    [SerializeField] GameObject Shadow;
    public List<GameObject> SelectCharacter = new List<GameObject>();
    public List<GameObject> SelectBuilding = new List<GameObject>();

    [SerializeField] GameObject Home;

    [SerializeField] GameObject Dialogue;

    public int MineCount;
    public GameObject TankMine;
    public GameObject HumanMine;

    public int TrenchCount;
    public int Trench;
    void Awake()
    {
        mainCam = Camera.main;
        GameManager.Instance.WallTile = GameObject.Find("Wall").GetComponent<Tilemap>();
        GameManager.Instance.PathFindTile = GameObject.Find("PathFindingWall").GetComponent<Tilemap>();
        //GameManager.Instance.BuildingTile.color = new Color(0, 0, 0, 0);
        GameManager.Instance.player = this;
        Shadow.GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, 1);
    }
    void Start()
    {
        GameObject OBJ = Instantiate(GameManager.Instance.Characterlist.characters[0].prefab, Home.transform.position, Quaternion.identity);
        GameManager.Instance.Character[0] = OBJ;
        GameManager.Instance.InBuilding(Home.transform.GetComponent<CharacterManager>(), OBJ.GetComponent<CharacterManager>());
        //Dialogue.SetActive(true);
    }
    void Update()
    {
        SelectMouse();
        MoveMouse();
        CameraMove();
        BuildingEnteraction();
        Enteraction();
    }
    void Enteraction()
    {
        if(Input.GetKeyDown(KeyCode.E))
        {
            for (int i = 0; i < SelectCharacter.Count; i++)
            {
                if (SelectCharacter[i].GetComponent<CharacterManager>().type == CharacterManager.Type.engineer)
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
    }
    void BuildingEnteraction()
    {
        if (Input.GetKeyDown(KeyCode.O))
        {
            for (int i = 0; i < SelectBuilding.Count; i++)
            {
                Building CM = SelectBuilding[i].GetComponent<Building>();
                if (CM.InPlayer)
                    GameManager.Instance.OutBuilding(CM, CM.transform.GetChild(CM.transform.childCount - 1).transform.GetComponent<CharacterManager>());
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
                    Tower tower = SelectCharacter[i].GetComponent<Tower>();
                    tower.TargetPos = mainCam.ScreenToWorldPoint(Input.mousePosition);
                    tower.Target = hit.transform.gameObject;
                    tower.StopCoroutine(tower.AttackCoroutine);
                    tower.StopCoroutine(tower.MoveCoroutine);
                    tower.ThatAttack(tower.Target);
                }
                for (int i = 0; i < SelectBuilding.Count; i++)
                {
                    Building building = SelectBuilding[i].GetComponent<Building>();
                    building.Target = hit.transform.gameObject;
                    building.StopCoroutine(building.AttackCoroutine);
                    building.ThatAttack(building.Target);
                }
                return;
            }
            for (int i = 0; i < SelectCharacter.Count; i++)
            {
                CharacterManager tower = SelectCharacter[i].GetComponent<Tower>();
                tower.StopCoroutine(tower.AttackCoroutine);
                tower.StopCoroutine(tower.MoveCoroutine);
                tower.TargetPos = mainCam.ScreenToWorldPoint(Input.mousePosition);
                SelectCharacter[i].transform.GetComponent<Tower>().PlayerOrder = true;
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
            SelectCharacter.Clear();
            SelectBuilding.Clear();
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
