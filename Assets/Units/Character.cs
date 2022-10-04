using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Character : MonoBehaviour, ISelectable {

	public int Health {
		get; private set;
	}

	const float minPathUpdateTime = .2f;
	const float pathUpdateMoveThreshold = .5f;

	public Transform target;
	public Vector3 targetOldPos;
	public float speed = 20f;
	public Vector3[] path;
	public int targetIndex;

	public bool executingCommand = false;

	private CommandManager commandManager;

	private Coroutine movementCoroutine;

	private Command currentCommand;

	public bool Embarked {
		get {
			return embarkedSeat != null;
		}
	}

	private IControllable embarkedSeat;

	GameObject[] children;

	private void Awake() {
		commandManager = GetComponent<CommandManager>();
		Health = 100;
	}

	private void Start () {
		StartCoroutine(UpdatePath());
	}

	private void Update() {
		
	}

	public void OnPathFound(Vector3[] newPath, bool pathSuccessful) {
		if (pathSuccessful) {
			path = newPath;
			if (movementCoroutine != null) StopCoroutine(movementCoroutine);
			movementCoroutine = StartCoroutine(FollowPath());
		}
	}

	public void Command_Move (Command command, Action<bool> callback) {
		MoveCommand move = command.GetAsType<MoveCommand>();
		Vector2 target = move.Target();

		PathRequestManager.RequestPath(transform.position, target, OnPathFound);

		callback(true);
	}

	public void Command_Interact (Command command, Action<bool> callback) {
		Debug.Log("Interact Commanded");
		IInteractable interactable = command.GetAsType<InteractCommand>().Target();

		target = interactable.GetObject().transform;
		targetOldPos = target.position;

		//Debug.Log(interactable.GetObject().name);

		PathRequestManager.RequestPath(transform.position, target.position, OnPathFound);

		currentCommand = command;

		callback(true);
	}

	//interaction trigger
	private void OnTriggerEnter2D (Collider2D collision) {
		//Debug.Log(currentCommand.GetType());

		if (currentCommand == null) return;

		bool isInteract = currentCommand.GetType() == typeof(InteractCommand);
		bool isInLayer = LayerMasks.IsInLayerMask(collision.gameObject.layer, LayerMasks.InteractableMask);

		//Debug.Log("Is Interaction: " + isInteract + "  |  Is Interactable Layer: " + isInLayer);

		if (isInteract && isInLayer) {
			collision.GetComponentInParent<IInteractable>().Interact(this);
			currentCommand = null;
			target = null;
		}
	}

	IEnumerator UpdatePath () {
		if (Time.timeSinceLevelLoad < .3f) {
			yield return new WaitForSeconds(.3f);
		}

		float sqrMoveThreshold = pathUpdateMoveThreshold * pathUpdateMoveThreshold;

		while (true) {
			yield return new WaitForSeconds(minPathUpdateTime);

			if (target != null && (target.position - targetOldPos).sqrMagnitude > sqrMoveThreshold) {
				PathRequestManager.RequestPath(transform.position, target.position, OnPathFound);
				targetOldPos = target.position;
			}
		}
	}

	IEnumerator FollowPath() {
		targetIndex = 0;
		Vector3 currentWaypoint = path[0];

		while (true) {
			if (transform.position == currentWaypoint) {
				targetIndex++;

				if (targetIndex >= path.Length) {
					// if (target != null) ;
					yield break;
				}

				currentWaypoint = path[targetIndex];
			}

			//Debug.Log(currentWaypoint);

			transform.position = Vector3.MoveTowards(transform.position, currentWaypoint, speed * Time.deltaTime);

			yield return null;
		}
	}

	public virtual void Embark (IControllable seat) {
		TankStation station = seat.GetObject().GetComponent<TankStation>();

		if (!station.Embark(this)) return;

		StopCoroutine(movementCoroutine);

		transform.SetParent(station.transform, true);

		transform.localPosition = Vector3.zero;

		//Debug.Log(transform.);

		embarkedSeat = seat;
	}

	public virtual void Disembark () {
		embarkedSeat.GetObject().GetComponent<TankStation>().Disembark();
		embarkedSeat = null;
		transform.position = transform.position + (Vector3.left * 2f);
		transform.SetParent(null);
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