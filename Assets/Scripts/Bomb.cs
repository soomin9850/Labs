using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    public GameObject Target;
    public void Click()
    {
        if(Target.CompareTag("Bridge"))
        {
            GameManager GM = GameManager.Instance;
            for (int i = 0; i < Target.transform.childCount; i++)
            {
                Vector3Int vector3 = new Vector3Int(Mathf.RoundToInt(Target.transform.GetChild(i).position.x), Mathf.RoundToInt(Target.transform.GetChild(i).position.y));
                GM.PathFindTile.SetTile(vector3, GM.TileBase);
            }
            Target.GetComponent<SpriteRenderer>().color = Color.red;
        }
        else if(Target.TryGetComponent(out Building building))
        {
            building.BuildingDie();
        }
        RaycastHit2D[] hits = Physics2D.CircleCastAll(Camera.main.ScreenToWorldPoint(Input.mousePosition), 5, transform.forward, 1, 1 << 6 | 1 << 7);
        for (int i = 0; i < hits.Length; i++)
        {
            hits[i].transform.GetComponent<CharacterManager>().HP -= 200;
        }
        Destroy(gameObject);
    }
}
