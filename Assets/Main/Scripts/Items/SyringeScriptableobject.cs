using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Syringe", menuName = "Syringe")]
[System.Serializable]
public class SyringeScriptableobject : ScriptableObject
{
    public float InstantHealing = 0;
    public float PermaDamageBoost = 0;
    public float TimeToUse = 1;
}
