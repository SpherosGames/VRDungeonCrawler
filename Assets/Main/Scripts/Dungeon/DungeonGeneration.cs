using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DungeonGeneration : MonoBehaviour{
    [Header("Dimensions")]
    [SerializeField] private int Width;
    [SerializeField] private int Height;

    [Header("Generation")]
    public List<DungeonNode> Nodes = new List<DungeonNode>();
    public List<Vector2Int> _ToCollapse = new List<Vector2Int>();
    private DungeonNode[,] _grid;

    private Vector2Int[] offsets = new Vector2Int[] {
        new Vector2Int(0, 1), // Top
        new Vector2Int(0, -1), // Bottom
        new Vector2Int(1, 0), // Right
        new Vector2Int(-1, 0), // Left
    };


    private void Start() {
        _grid = new DungeonNode[Width, Height];
        _CollapseInto();
    }

    private void _CollapseInto() {
        _ToCollapse.Clear();
        _ToCollapse.Add(new Vector2Int(Width / 2, Height / 2));

        while (_ToCollapse.Count > 0) {
            int x = _ToCollapse[0].x;
            int y = _ToCollapse[0].y;

            List<DungeonNode> PotentialNodes = new List<DungeonNode>(Nodes);

            for (int i = 0; i < offsets.Length; i++) {
                Vector2Int Neighbour = new Vector2Int(x + offsets[i].x, y + offsets[i].y);
                if (IsInsideGrid(Neighbour)) {
                    DungeonNode NeighbourNode = _grid[Neighbour.x, Neighbour.y];

                    if (NeighbourNode != null) {
                        WhittleNodes(PotentialNodes);
                        break;
                    } else {
                        if (!_ToCollapse.Contains(Neighbour)) _ToCollapse.Add(Neighbour);
                    }
                }
            }

            if (PotentialNodes[0] == null) {
                _grid[x, y] = Nodes[0];
                Debug.Log("Attempted to collapse wave on " + x + ", " + y + " but found no compatible nodes.");
            } else {
                _grid[x, y] = PotentialNodes[0];
            }

            GameObject NewNode = Instantiate(_grid[x, y].Prefab, new Vector3(x * 13.3f, 0f, y * 13.3f), Quaternion.identity);
            _ToCollapse.RemoveAt(0);
        }
    }

    private void WhittleNodes(List<DungeonNode> PotentialNodes) {
        int randomVal = Random.Range(1, PotentialNodes.Count);

        PotentialNodes.RemoveRange(0, PotentialNodes.Count - randomVal);
        PotentialNodes.RemoveRange(1, PotentialNodes.Count-1);
    }

    private bool IsInsideGrid(Vector2Int v2int) {
        if (v2int.x > -1 && v2int.x < Width && v2int.y > -1 && v2int.y < Height) {
            return true;
        }
        else return false;
    }
}
