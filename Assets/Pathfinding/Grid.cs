using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Grid : MonoBehaviour {

    public Vector2Int GridSize;

    public bool displayGizmos;

    public TerrainType[] walkableRegions;

    [SerializeField]
    private float nodeSize = 1;

    public GameObject agent;
    public GameObject target;

    private Node[,] grid;

    [SerializeField]
    private Pathfinding pathfinding;

    public int MaxGridSize {
        get {
            return GridSize.x * GridSize.y;
		}
	}

    private Vector3 BottomLeft {
        get {
            return new Vector3(transform.position.x - nodeSize * GridSize.x/2, transform.position.y - nodeSize * GridSize.y/2);
        }
	}

	private void Awake() {
        pathfinding = GetComponent<Pathfinding>();
        CreateGrid();
    }

	private void CreateGrid () {
        grid = new Node[GridSize.x, GridSize.y];

        for (int x = 0; x < GridSize.x; x++) {
            for (int y = 0; y < GridSize.y; y++) {
                Vector3 worldPos = BottomLeft + new Vector3(x * nodeSize + (nodeSize/2), y * nodeSize + (nodeSize / 2), 0);

                int modifier = Physics2D.BoxCast(worldPos, Vector2.one * (nodeSize / 2), 0, Vector2.zero).collider == null ? 100 : 0;

                grid[x, y] = new Node(x, y, worldPos, modifier);
            }
        }
    }

    public List<Node> GetNeighbours (Node node) {
        List<Node> neighbours = new List<Node>();

        for (int x = -1; x <= 1; x++) {
            for (int y = -1; y <= 1; y++) {
                if (x == 0 && y == 0) continue;

                int checkX = node.GridPos.x + x;
                int checkY = node.GridPos.y + y;

                if (checkX >= 0 && checkX < GridSize.x && checkY >= 0 && checkY < GridSize.y) {
					neighbours.Add(grid[checkX, checkY]);
				}
            }
		}

        return neighbours;
	}

	public Node GetNodeFromWorldPos (Vector3 worldPos) {
        //Debug.Log("GetNode called");
        float percentX = (worldPos.x - BottomLeft.x) / (GridSize.x * nodeSize);
        float percentY = (worldPos.y - BottomLeft.y) / (GridSize.y * nodeSize);
        //Debug.Log("Percent values calculated");
        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);
        //Debug.Log("Percent values clamped");
        int x = Mathf.RoundToInt((GridSize.x - 1) * percentX);
        int y = Mathf.RoundToInt((GridSize.y - 1) * percentY);
        //Debug.Log("Values rounded to int");
        return grid[x, y];
    }

	private void OnDrawGizmos() {
        if (displayGizmos) {
            Gizmos.DrawWireCube(transform.position, new Vector3(GridSize.x * nodeSize, GridSize.y * nodeSize, 1));

            if (grid != null) {

                foreach (Node n in grid) {
                    Gizmos.color = Color.white;

                    if (!n.Walkable) Gizmos.color = Color.red;

                    Gizmos.DrawWireCube(n.Position, Vector3.one * nodeSize);
                }
            }
        }
	}

	[System.Serializable]
    public class TerrainType {
        public LayerMask terrainMask;
        public int terrainPenalty;
	}
}