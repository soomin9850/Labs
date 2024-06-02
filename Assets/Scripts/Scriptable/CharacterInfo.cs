using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Character", menuName = "CharacterInfo", order = int.MaxValue)]
public class CharacterInfo : ScriptableObject
{
    public Character[] characters;
}
[Serializable]
public class Character
{
    public string name;
    public int ID;

    public GameObject prefab;
    public HP hp;
    public Speed speed;
    public Attack attack;
    public AttackSpeed attackSpeed;
    public AntiAmor antiAmor;
    public Sight sight;
}
[Serializable]
public struct HP
{
    public int MaxLevel;
    public int Increase;
    public int Level;
}
[Serializable]
public struct Speed
{
    public int MaxLevel;
    public int Increase;
    public int Level;
}
[Serializable]
public struct Attack
{
    public int MaxLevel;
    public int Increase;
    public int Level;
}
[Serializable]
public struct AttackSpeed
{
    public int MaxLevel;
    public int Increase;
    public int Level;
}
[Serializable]
public struct AntiAmor
{
    public int MaxLevel;
    public int Increase;
    public int Level;
}
[Serializable]
public struct Sight
{
    public int MaxLevel;
    public int Increase;
    public int Level;
}