using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SocketableType
{
    keyLock,
    Test,
    Cork
}

[RequireComponent(typeof(Grabbable))]
public class Socketable : MonoBehaviour
{
    [SerializeField] private SocketableType socket;

    public SocketableType GetSocketableType() => socket;
}
