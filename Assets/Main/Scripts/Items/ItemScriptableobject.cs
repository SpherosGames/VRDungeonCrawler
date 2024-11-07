using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Item")]
[System.Serializable]
public class ItemScriptableobject : ScriptableObject
{
    public GameObject Prefab;
    public int Weight;
    public Gradient TrailGradient;

    [Header("Only use if item is syringe!")]
    public SyringeScriptableobject SyringeType;
}
