using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinding : MonoBehaviour {

	public PathRequestManager requestManager;
	public Grid grid;

	private void Awake() {
		//requestManager = GetComponent<PathRequestManager>();
		//grid = GetComponent<Grid>();
	}

	public void StartFindPath (Vector3 startPos, Vector3 targetPos) {
		StartCoroutine(FindPath(startPos, targetPos));
	}

	IEnumerator FindPath (Vector3 originPos, Vector3 targetPos) {
		Vector3[] waypoints = new Vector3[0];
		bool pathSuccess = false;

		Node originNode = grid.GetNodeFromWorldPos(originPos);
		Node targetNode = grid.GetNodeFromWorldPos(targetPos);

		if (originNode.Walkable && targetNode.Walkable) {
			Heap<Node> openSet = new Heap<Node>(grid.MaxGridSize);
			HashSet<Node> closedSet = new HashSet<Node>();
			openSet.Add(originNode);

			while (openSet.Count > 0) {
				Node currentNode = openSet.RemoveFirst();
				closedSet.Add(currentNode);

				if (currentNode == targetNode) {
					pathSuccess = true;
					break;
				}

				foreach (Node neighbour in grid.GetNeighbours(currentNode)) {
					if (neighbour.Modifier == 0 || closedSet.Contains(neighbour)) continue;

					int newMoveCost = currentNode.GCost + GetDistance(currentNode, neighbour);

					if (newMoveCost < neighbour.GCost || !openSet.Contains(neighbour)) {
						neighbour.GCost = newMoveCost;
						neighbour.HCost = GetDistance(neighbour, targetNode);
						neighbour.parent = currentNode;

						if (!openSet.Contains(neighbour)) openSet.Add(neighbour);
						else openSet.UpdateItem(neighbour);
					}
				}
			}

			yield return null;

			if (pathSuccess) {
				waypoints = RetracePath(originNode, targetNode);
			}

			requestManager.FinishedProcessingPath(waypoints, pathSuccess);
		}
	}

	private Vector3[] RetracePath (Node originNode, Node endNode) {
		List<Node> path = new List<Node>();

		Node currentNode = endNode;

		while (currentNode != originNode) {
			path.Add(currentNode);
			currentNode = currentNode.parent;
		}

		Vector3[] waypoints = SimplifyPath(path);

		Array.Reverse(waypoints);

		return waypoints;
	}

	private Vector3[] SimplifyPath (List<Node> path) {
		List<Vector3> waypoints = new List<Vector3>();

		Vector2 directionOld = Vector2.zero;

		for (int i = 1; i < path.Count; i++) {
			Vector2 directionNew = new Vector2(path[i-1].GridPos.x - path[i].GridPos.x, path[i - 1].GridPos.y - path[i].GridPos.y);

			if (directionNew != directionOld) {
				waypoints.Add(path[i].Position);
			}

			directionOld = directionNew;
		}

		return waypoints.ToArray();
	}

	private int GetDistance (Node originNode, Node targetNode) {
		int distX = Mathf.Abs(originNode.GridPos.x - targetNode.GridPos.x);
		int distY = Mathf.Abs(originNode.GridPos.y - targetNode.GridPos.y);

		if (distX > distY) return 14 * distY + 10 * (distX - distY);
		return 14 * distX + 10 * (distY - distX);
	}
}