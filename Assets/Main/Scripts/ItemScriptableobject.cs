using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Item")]
[System.Serializable]
public class ItemScriptableobject : ScriptableObject
{
    public GameObject prefab;
    public int weight;
}
