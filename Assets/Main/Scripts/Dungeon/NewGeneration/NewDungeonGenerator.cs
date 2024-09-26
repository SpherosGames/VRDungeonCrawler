using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewDungeonGenerator : MonoBehaviour {
    public static NewDungeonGenerator Instance { get; private set; }

    [SerializeField] private GameObject Entrance;

    [Header("Room Generation")]
    [SerializeField] private List<GameObject> Rooms;
    [SerializeField] private List<GameObject> SpecialRooms;
    [SerializeField] private List<GameObject> AlternateEntrances;
    [SerializeField] private List<GameObject> Hallways;

    [Header("Filler Objects")]
    [SerializeField] private GameObject Door;

    [Header("Variables")]
    [SerializeField] private int NumOfRooms = 10;
    [SerializeField] private LayerMask RoomsLayerMask;
    [SerializeField] private List<DungeonPart> GeneratedRooms;

    private bool IsGenerated;

    private void Awake() {
        Instance = this;
    }

    private void Start() {
        _Generate();
        _GenerateAlternateEntrances();
        _FillEmptyEntrances();

        IsGenerated = true;
    }

    private void _Generate()
    {
        for (int i = 0; i < NumOfRooms - AlternateEntrances.Count; i++) {
            if (GeneratedRooms.Count < 1) {
                GameObject GeneratedRoom = Instantiate(Entrance, transform.position, transform.rotation);
                GeneratedRoom.transform.SetParent(null);

                if (GeneratedRoom.TryGetComponent<DungeonPart>(out DungeonPart _DungeonPart)) {
                    GeneratedRooms.Add(_DungeonPart);
                }
            } else {
                bool ShouldPlaceHallway = Random.Range(0f, 1f) > 0.5f;
                DungeonPart RandomGeneratedRoom = null;
                Transform Room1EntryPoint = null;

                int TotalRetries = 100;
                int RetryIndex = 0;

                while (RandomGeneratedRoom == null && RetryIndex < TotalRetries) {
                    int RandomLinkRoomIndex = Random.Range(0, GeneratedRooms.Count);
                    DungeonPart RoomToTest = GeneratedRooms[RandomLinkRoomIndex];

                    if (RoomToTest.HasAvailableEntryPoint(out Room1EntryPoint)) {
                        RandomGeneratedRoom = RoomToTest;
                        break;
                    }
                    RetryIndex++;
                }

                GameObject DoorToAlign = Instantiate(Door, transform.position, transform.rotation);
                //DoorToAlign.transform.SetParent(null);

                if (ShouldPlaceHallway) {
                    int RandomIndex = Random.Range(0, Hallways.Count);
                    GameObject GeneratedHallway = Instantiate(Hallways[RandomIndex], transform.position, transform.rotation);
                    GeneratedHallway.transform.SetParent(null);
                    
                    if (GeneratedHallway.TryGetComponent<DungeonPart>(out DungeonPart _DungeonPart)) {
                        if (_DungeonPart.HasAvailableEntryPoint(out Transform _Room2EntryPoint)) {
                            GeneratedRooms.Add(_DungeonPart);

                            DoorToAlign.transform.position = Room1EntryPoint.transform.position;
                            DoorToAlign.transform.rotation = Room1EntryPoint.transform.rotation;

                            AlignRooms(RandomGeneratedRoom.transform, GeneratedHallway.transform, Room1EntryPoint, _Room2EntryPoint);

                            if (HandleIntersection(_DungeonPart)) {
                                _DungeonPart.UnuseEntrypoint(_Room2EntryPoint);
                                RandomGeneratedRoom.UnuseEntrypoint(Room1EntryPoint);
                                RetryPlacement(GeneratedHallway, DoorToAlign);
                                continue;
                            }
                        }
                    }
                } else {
                    GameObject GeneratedRoom;

                    if (SpecialRooms.Count > 0) {
                        bool ShouldPlaceSpecialRoom = Random.Range(0f, 1f) > 0.9f;
                        if (ShouldPlaceSpecialRoom) { 
                            int RandomIndex = Random.Range(0, SpecialRooms.Count);
                            GeneratedRoom = Instantiate(SpecialRooms[RandomIndex], transform.position, transform.rotation);
                        } else { 
                            int RandomIndex = Random.Range(0, Rooms.Count);
                            GeneratedRoom = Instantiate(Rooms[RandomIndex], transform.position, transform.rotation);
                        }
                    } else {
                        int RandomIndex = Random.Range(0, Rooms.Count);
                        GeneratedRoom = Instantiate(Rooms[RandomIndex], transform.position, transform.rotation);
                    }

                    GeneratedRoom.transform.SetParent(null);
                    if (GeneratedRoom.TryGetComponent<DungeonPart>(out DungeonPart _DungeonPart)) { 
                        if (_DungeonPart.HasAvailableEntryPoint(out Transform Room2EntryPoint)) { 
                            GeneratedRooms.Add(_DungeonPart);

                            DoorToAlign.transform.position = Room1EntryPoint.transform.position;
                            DoorToAlign.transform.rotation = Room1EntryPoint.transform.rotation;
                            AlignRooms(RandomGeneratedRoom.transform, GeneratedRoom.transform, Room1EntryPoint, Room2EntryPoint);

                            if (HandleIntersection(_DungeonPart)) {
                                _DungeonPart.UnuseEntrypoint(Room2EntryPoint);
                                RandomGeneratedRoom.UnuseEntrypoint(Room1EntryPoint);
                                RetryPlacement(GeneratedRoom, DoorToAlign);
                                continue;
                            }
                        }
                    }
                }
            }
        }
    }
    private void _GenerateAlternateEntrances() {
        if (AlternateEntrances.Count < 1) return;
    }

    private void _FillEmptyEntrances() {
        GeneratedRooms.ForEach(Room => Room.FillEmptyDoors());
    }

    private void RetryPlacement(GameObject ItemToPlace, GameObject DoorToPlace) {
        DungeonPart RandomGeneratedRoom = null;
        Transform Room1EntryPoint = null;
        int TotalRetries = 100;
        int RetryIndex = 0;

        while (RandomGeneratedRoom == null && RetryIndex < TotalRetries) {
            int RandomLinkRoomIndex = Random.Range(0, GeneratedRooms.Count - 1);
            DungeonPart RoomToTest = GeneratedRooms[RandomLinkRoomIndex];

            if (RoomToTest.HasAvailableEntryPoint(out Room1EntryPoint)) {
                RandomGeneratedRoom = RoomToTest;
                break;
            }
            RetryIndex++;
        }

        if (ItemToPlace.TryGetComponent<DungeonPart>(out DungeonPart _DungeonPart)) {
            if (_DungeonPart.HasAvailableEntryPoint(out Transform Room2EntryPoint)) {
                DoorToPlace.transform.position = Room1EntryPoint.transform.position;
                DoorToPlace.transform.rotation = Room1EntryPoint.transform.rotation;

                AlignRooms(RandomGeneratedRoom.transform, ItemToPlace.transform, Room1EntryPoint, Room2EntryPoint);

                if (HandleIntersection(_DungeonPart)) {
                    _DungeonPart.UnuseEntrypoint(Room2EntryPoint);
                    RandomGeneratedRoom.UnuseEntrypoint(Room1EntryPoint);
                    RetryPlacement(ItemToPlace, DoorToPlace);
                }
            }
        }
    }

    private bool HandleIntersection(DungeonPart _DungeonPart) {
        bool DidIntersect = false;

        Collider[] hits = Physics.OverlapBox(_DungeonPart._Collider.bounds.center, _DungeonPart._Collider.bounds.size / 2, Quaternion.identity, RoomsLayerMask);
        foreach (Collider hit in hits) {
            if (hit == _DungeonPart._Collider) continue;
            if (hit != _DungeonPart._Collider) {
                DidIntersect = true; break;
            }
        }

        return DidIntersect;
    }

    private void AlignRooms(Transform Room1, Transform Room2, Transform Room1Entry, Transform Room2Entry) {
        float angle = Vector3.Angle(Room1Entry.forward, Room2Entry.forward);

        Room2.TransformPoint(Room2Entry.position);
        Room2.eulerAngles = new Vector3(Room2.eulerAngles.x, Room2.eulerAngles.y + angle, Room2.eulerAngles.z);

        Vector3 offset = Room1Entry.position - Room2Entry.position;
        Room2.position += offset;

        Physics.SyncTransforms();
    }
}
