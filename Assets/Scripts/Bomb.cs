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

        }
        else if(TryGetComponent(out Building component))
        {
            component.BuildingDie();
        }
    }
}
