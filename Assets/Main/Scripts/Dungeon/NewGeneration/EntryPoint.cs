using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntryPoint : MonoBehaviour {
    private bool _IsOccupied = false;
    public void SetOccupied(bool value = true) => _IsOccupied = value;
    public bool IsOccupied() => _IsOccupied;
}
