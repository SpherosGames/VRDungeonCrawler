using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Syringe", menuName = "Syringe")]
[System.Serializable]
public class SyringeScriptableobject : ScriptableObject
{
    [Tooltip("Healing the player gets from using the item")]
    public float InstantHealing = 0;

    [Tooltip("Permanent damage increase")]
    public float PermaDamageBoost = 0;

    [Tooltip("The time until the effects take into use and the time until it drops the syringe out of you")]
    public float TimeToUse = 1;
}
