using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour, ISelectable {

	const float minPathUpdateTime = .2f;
	const float pathUpdateMoveThreshold = .5f;

	public Transform target;
	public float speed = 20f;
	Vector3[] path;
	int targetIndex;

	private void Start () {
		StartCoroutine(UpdatePath());
	}

	public void OnPathFound(Vector3[] newPath, bool pathSuccessful) {
		if (pathSuccessful) {
			path = newPath;
			StopCoroutine(FollowPath());
			StartCoroutine(FollowPath());
		}
	}

	IEnumerator UpdatePath () {
		if (Time.timeSinceLevelLoad < .3f) {
			yield return new WaitForSeconds(.3f);
		}
		PathRequestManager.RequestPath(new PathRequest(transform.position, target.position, OnPathFound));

		float sqrMoveThreshold = pathUpdateMoveThreshold * pathUpdateMoveThreshold;
		Vector3 targetPosOld = target.position;

		while (true) {
			yield return new WaitForSeconds(minPathUpdateTime);
			if ((target.position - targetPosOld).sqrMagnitude > sqrMoveThreshold) {
				PathRequestManager.RequestPath(new PathRequest(transform.position, target.position, OnPathFound));
				targetPosOld = target.position;
			}
		}
	}

	IEnumerator FollowPath() {
		Vector3 currentWaypoint = path[0];

		while (true) {
			if (transform.position == currentWaypoint) {
				targetIndex++;

				if (targetIndex >= path.Length) {
					yield break;
				}

				currentWaypoint = path[targetIndex];
			}

			transform.position = Vector3.MoveTowards(transform.position, currentWaypoint, speed * Time.deltaTime);
			yield return null;
		}
	}

	public void OnDrawGizmos() {
		if (path != null) {
			for (int i = targetIndex; i < path.Length; i++) {
				Gizmos.color = Color.black;
				Gizmos.DrawCube(path[i], Vector3.one/2);

				if (i == targetIndex) {
					Gizmos.DrawLine(transform.position, path[i]);
				}
				else {
					Gizmos.DrawLine(path[i - 1], path[i]);
				}
			}
		}
	}
}