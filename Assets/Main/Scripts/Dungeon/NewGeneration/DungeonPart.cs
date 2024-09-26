using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonPart: MonoBehaviour {
    public enum DungeonPartType {
        Room,
        Hallway,
    }

    [SerializeField] private LayerMask RoomsLayerMask;
    [SerializeField] private DungeonPartType _DungeonPartType;
    [SerializeField] private GameObject FillerWall;
    
    public List<Transform> EntryPoints;
    public new Collider _Collider;

    public bool HasAvailableEntryPoint(out Transform EntryPoint) {
        Transform ResultingEntry = null;
        bool result = false;

        int TotalRetries = 100;
        int RetryIndex = 0;

        if (EntryPoints.Count == 1) {
            Transform Entry = EntryPoints[0];
            if (Entry.TryGetComponent<EntryPoint>(out EntryPoint res)) {
                if (res.IsOccupied()) {
                    result = false;
                    ResultingEntry = null;
                } else {
                    result = true;
                    ResultingEntry = Entry;
                    res.SetOccupied();
                }
                EntryPoint = ResultingEntry;
                return result;
            }
        }

        while (ResultingEntry == null && RetryIndex < TotalRetries) {
            int RandomEntryIndex = Random.Range(0, EntryPoints.Count);
            Transform Entry = EntryPoints[RandomEntryIndex];

            if (Entry.TryGetComponent<EntryPoint>(out EntryPoint _EntryPoint)) {
                if (!_EntryPoint.IsOccupied()) {
                    ResultingEntry = Entry;
                    result = true;
                    _EntryPoint.SetOccupied();
                    break;
                }
            }
            RetryIndex++;
        }

        EntryPoint = ResultingEntry;
        return result;
    }

    public void UnuseEntrypoint(Transform EntryPoint) {
        if (EntryPoint.TryGetComponent<EntryPoint>(out EntryPoint entry)) {
            entry.SetOccupied();
        }
    }

    public void FillEmptyDoors() {
        EntryPoints.ForEach((Entry) => {
            if (Entry.TryGetComponent(out EntryPoint _EntryPoint)) {
                if (!_EntryPoint.IsOccupied()) {
                    GameObject Wall = Instantiate(FillerWall);
                    Wall.transform.position = Entry.transform.position;
                    Wall.transform.rotation = Entry.transform.rotation;
                }
            }
        });
    }

    void OnDrawGizmos() {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(_Collider.bounds.center, _Collider.bounds.size);
    }
}
