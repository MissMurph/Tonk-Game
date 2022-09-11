using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinding : MonoBehaviour {

	Grid grid;

	private void Awake() {
		grid = GetComponent<Grid>();
	}

	public void FindPath (Vector3 originPos, Vector3 targetPos) {
		Node originNode = grid.GetNodeFromWorldPos(originPos);
		Node targetNode = grid.GetNodeFromWorldPos(targetPos);

		List<Node> openSet = new List<Node>();
		HashSet<Node> closedSet = new HashSet<Node>();
		openSet.Add(originNode);

		while (openSet.Count > 0) {
			Node currentNode = openSet[0];

			for (int i = 1; i < openSet.Count; i++) {
				if (openSet[i].FCost < currentNode.FCost || openSet[i].FCost == currentNode.FCost && openSet[i].HCost < currentNode.HCost) {
					currentNode = openSet[i];
				}
			}

			openSet.Remove(currentNode);
			closedSet.Add(currentNode);

			if (currentNode == targetNode) {
				grid.path = RetracePath(originNode, currentNode);
				return;
			}

			foreach (Node neighbour in grid.GetNeighbours(currentNode)) {
				if (neighbour.Modifier == 0 || closedSet.Contains(neighbour)) continue;

				int newMoveCost = currentNode.GCost + GetDistance(currentNode, neighbour);

				if (newMoveCost < neighbour.GCost || !openSet.Contains(neighbour)) {
					neighbour.GCost = newMoveCost;
					neighbour.HCost = GetDistance(neighbour, targetNode);
					neighbour.parent = currentNode;

					if (!openSet.Contains(neighbour)) {
						openSet.Add(neighbour);
					}
				}
			}
		}
	}

	private List<Node> RetracePath (Node originNode, Node endNode) {
		List<Node> path = new List<Node>();

		Node currentNode = endNode;

		while (currentNode != originNode) {
			path.Add(currentNode);
			currentNode = currentNode.parent;
		}

		path.Reverse();

		return path;
	}

	private int GetDistance (Node originNode, Node targetNode) {
		int distX = Mathf.Abs(originNode.GridPos.x - targetNode.GridPos.x);
		int distY = Mathf.Abs(originNode.GridPos.y - targetNode.GridPos.y);

		if (distX > distY) return 14 * distY + 10 * (distX - distY);
		return 14 * distX + 10 * (distY - distX);
	}
}