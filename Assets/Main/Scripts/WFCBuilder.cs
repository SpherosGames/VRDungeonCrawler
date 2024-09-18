using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WFCBuilder : MonoBehaviour {
    [SerializeField] private int Width;
    [SerializeField] private int Height;

    private WFCNode[,] _grid;
    
    public List<WFCNode> Nodes = new List<WFCNode>();

    public List<Vector2Int> _ToCollapse = new List<Vector2Int>();

    private Vector2Int[] offsets = new Vector2Int[] {
        new Vector2Int(0, 1), // Top
        new Vector2Int(0, -1), // Bottom
        new Vector2Int(1, 0), // Right
        new Vector2Int(-1, 0), // Left
    };

    private void Start()
    {
        _grid = new WFCNode[Width, Height];
        CollapseWorld();
    }

    private void CollapseWorld() {
        _ToCollapse.Clear();
        _ToCollapse.Add(new Vector2Int(Width / 2, Height / 2));

        while (_ToCollapse.Count > 0) {
            int x = _ToCollapse[0].x;
            int y = _ToCollapse[0].y;

            List<WFCNode> PotentialNodes = new List<WFCNode>(Nodes);

            for (int i = 0; i < offsets.Length; i++) {
                Vector2Int Neighbour = new Vector2Int(x + offsets[i].x, y + offsets[i].y);

                if (IsInsideGrid(Neighbour)) {
                    WFCNode NeighbourNode = _grid[Neighbour.x, Neighbour.y];

                    if (NeighbourNode != null) {
                        switch (i) {
                            case 0:
                                WhittleNodes(PotentialNodes, NeighbourNode.Bottom.CompatibleNodes);
                                break;
                            case 1:
                                WhittleNodes(PotentialNodes, NeighbourNode.Top.CompatibleNodes);
                                break;
                            case 2:
                                WhittleNodes(PotentialNodes, NeighbourNode.Left.CompatibleNodes);
                                break;
                            case 3:
                                WhittleNodes(PotentialNodes, NeighbourNode.Right.CompatibleNodes);
                                break;

                        }
                    }
                    else {
                        if (!_ToCollapse.Contains(Neighbour)) _ToCollapse.Add(Neighbour);
                    }
                }
            }

            if (PotentialNodes.Count < 1) {
                _grid[x, y] = Nodes[0];
                Debug.Log("Attempted to collapse wave on " + x + ", " + y + " but found no compatible nodes.");
            } else {
                _grid[x, y] = PotentialNodes[Random.Range(0, PotentialNodes.Count)];
            }

            GameObject NewNode = Instantiate(_grid[x, y].Prefab, new Vector3(x, 0f, y), Quaternion.identity);
            _ToCollapse.RemoveAt(0);
        }
    }

    private void WhittleNodes(List<WFCNode> PotentialNodes, List<WFCNode> ValidNodes) {
        for (int i = 0; i < PotentialNodes.Count; i++) {
            if (!ValidNodes.Contains(PotentialNodes[i])) {
                PotentialNodes.RemoveAt(i);
            }
        }
    }

    private bool IsInsideGrid(Vector2Int v2int) {
        if (v2int.x > -1 && v2int.x < Width && v2int.y > -1 && v2int.y < Height) {
            return true;
        }
        else return false;
    }
}
