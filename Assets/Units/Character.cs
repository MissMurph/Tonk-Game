using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Character : MonoBehaviour, ISelectable {

	const float minPathUpdateTime = .2f;
	const float pathUpdateMoveThreshold = .5f;

	public Transform target;
	public float speed = 20f;
	Vector3[] path;
	int targetIndex;

	public bool executingCommand = false;

	private CommandManager commandManager;

	float commandUpdateTime = 0.5f;
	float currentCommandTime = 0.5f;

	private void Awake() {
		commandManager = GetComponent<CommandManager>();
	}

	private void Start () {

	}

	private void Update() {
		
	}

	public void OnPathFound(Vector3[] newPath, bool pathSuccessful) {
		
	}

	public void Command_Move (Command command, Action<bool> callback) {
		Vector2 target = command.GetAsType<MoveCommand>().Target();

		PathRequestManager.RequestPath(transform.position, target, (newPath, pathSuccessful) => {
			if (pathSuccessful) {
				path = newPath;
				targetIndex = 0;
				StopCoroutine(FollowPath());
				StartCoroutine(FollowPath());
			}
		});
	}

	IEnumerator UpdatePath () {
		if (Time.timeSinceLevelLoad < .3f || target == null) {
			yield return new WaitForSeconds(.3f);
		}
		PathRequestManager.RequestPath(transform.position, target.position, OnPathFound);

		float sqrMoveThreshold = pathUpdateMoveThreshold * pathUpdateMoveThreshold;
		Vector3 targetPosOld = target.position;

		while (true) {
			yield return new WaitForSeconds(minPathUpdateTime);
			if ((target.position - targetPosOld).sqrMagnitude > sqrMoveThreshold) {
				PathRequestManager.RequestPath(transform.position, target.position, OnPathFound);
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
					executingCommand = false;
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

	/*	INTERFACE FUNCTIONS	*/

	public void EnqueueCommand (Command command) {
		commandManager.EnqueueCommand(command);
	}

	public void ExecuteCommand(Command command) {
		if (command != null) commandManager.ExecuteCommand(command);
		else Debug.Log("Null Command");
	}

	public GameObject GetObject() {
		return this.gameObject;
	}

	public ISelectable Select () {
		return this;
	}
}