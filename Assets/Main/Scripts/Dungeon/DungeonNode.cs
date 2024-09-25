using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public enum directionTypes {
    none,
    door,
    open,
    wall,
}


[CreateAssetMenu(fileName = "DungeonNode", menuName = "Dungeon/Node")]
[System.Serializable]
public class DungeonNode : ScriptableObject {
    public string Name;
    public GameObject Prefab;

    public directionTypes Top = directionTypes.none;
    public directionTypes Bottom = directionTypes.none;
    public directionTypes Left = directionTypes.none;
    public directionTypes Right = directionTypes.none;
}