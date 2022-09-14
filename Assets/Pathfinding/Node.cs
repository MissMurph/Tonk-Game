using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : IHeapItem<Node> {

    public int HeapIndex { get; set; }

    public Vector2Int GridPos { get; private set; }
    public Vector3 Position { get; private set; }
    public int Modifier { get; private set; }

    public int FCost { get { return GCost + HCost; } }
    public int GCost { get; set; }
    public int HCost { get; set; }
    public bool Walkable { get { return Modifier > 0; } }

    public Node parent;

    public Node(int _x, int _y, Vector3 _position, int _modifier) {
        GridPos = new Vector2Int(_x, _y);
        Modifier = _modifier;
        Position = _position;
    }

    public int CompareTo (Node nodeToCompare) {
        int compare = FCost.CompareTo(nodeToCompare.FCost);

        if (compare == 0) {
            compare = HCost.CompareTo(nodeToCompare.HCost);
		}

        return -compare;
	}
} 