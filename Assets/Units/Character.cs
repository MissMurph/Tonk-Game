using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TankGame.Units.Commands;
using TankGame.Players.Input;
using TankGame.Tanks.Stations;
using TankGame.Items;
using TankGame.Units.Interactions;
using TankGame.Tanks;
using TankGame.Units.Ai;
using TankGame.Units.Navigation;

namespace TankGame.Units {

	public class Character : Unit, ISelectable {

		[SerializeField]
		public int Health { get; private set; } = 100;
		[SerializeField]
		public int Morale { get; private set; } = 75;
		[SerializeField]
		public int Stress { get; private set; } = 0;

		const float minPathUpdateTime = .2f;
		const float pathUpdateMoveThreshold = .5f;

		public Transform target;
		public Vector3 targetOldPos;
		public float speed = 20f;
		public Vector3[] path;
		public int targetIndex;

		public bool executingCommand = false;

		public InteractionManager IntManager { get; private set; }

		private Coroutine movementCoroutine;

		public delegate void PathComplete(bool success);

		private PathComplete pathCompleteCallback;

		public Tank EmbarkedVehicle { get; protected set; }

		private StateMachine stateMachine;

		public ITraversable Traversable;

		[SerializeField]
		private Transform targetTracker;

		public bool Embarked {
			get {
				return EmbarkedVehicle != null;
			}
		}

		public Seat EmbarkedSeat { get; protected set; }

		public bool Seated {
			get {
				return EmbarkedSeat != null;
			}
		}

		private void Awake() {
			IntManager = GetComponent<InteractionManager>();
			stateMachine = GetComponent<StateMachine>();
			//Health = 100;
		}

		private void Start() {
			Traversable = GetComponentInParent<ITraversable>();
			StartCoroutine(UpdatePath());
		}

		private void OnPathFound(Vector3[] newPath, bool pathSuccessful) {
			if (pathSuccessful) {
				path = newPath;
				if (movementCoroutine != null) StopCoroutine(movementCoroutine);
				movementCoroutine = StartCoroutine(FollowPath());
			}
		}

		//This will assume that the target exists only in the global Traversable and will not be usable for Transforms within a Traversable
		public void SubmitTarget(Vector2 _target, PathComplete callback) {
			targetTracker.transform.position = _target;
			RequestPath(targetTracker, Navigation.World.GlobalTraversable, callback);
		}

		//Use this if need to potentially cross Traversables
		public void SubmitTarget(Transform _target, PathComplete callback) {
			RequestPath(_target, Traversable, callback);
		}

		private void RequestPath (Transform _target, ITraversable _traversable, PathComplete callback) {
			PathRequestManager.RequestPath(transform, _target, _traversable, OnPathFound);
			pathCompleteCallback = callback;
			target = _target;
		}

		public void Stop () {
			if (movementCoroutine != null) StopCoroutine(movementCoroutine);
			//movementCoroutine = StartCoroutine(FollowPath());
			path = null;
			target = null;
		}

		IEnumerator UpdatePath() {
			if (Time.timeSinceLevelLoad < .3f) {
				yield return new WaitForSeconds(.3f);
			}

			float sqrMoveThreshold = pathUpdateMoveThreshold * pathUpdateMoveThreshold;

			while (true) {
				yield return new WaitForSeconds(minPathUpdateTime);

				if (target != null && (target.position - targetOldPos).sqrMagnitude > sqrMoveThreshold) {
					PathRequestManager.RequestPath(transform, target, Traversable, OnPathFound);
					targetOldPos = target.position;
				}
			}
		}

		IEnumerator FollowPath() {
			targetIndex = 0;
			Vector3 currentWaypoint = path[0];

			while (true) {
				//if (CommManager.ActiveCommand != null && CommManager.ActiveCommand.Phase != Command.CommandPhase.Started) yield return null;

				if (transform.position == currentWaypoint) {
					targetIndex++;

					if (targetIndex >= path.Length) {
						// if (target != null) ;
						pathCompleteCallback.Invoke(true);
						yield break;
					}

					currentWaypoint = path[targetIndex];
				}

				//Debug.Log(currentWaypoint);

				transform.localPosition = Vector3.MoveTowards(transform.position, currentWaypoint, speed * Time.deltaTime);

				yield return null;
			}
		}

		/*public virtual void Embark(IControllable seat) {
			TankStation station = seat.GetObject().GetComponent<TankStation>();

			if (!station.Embark(this)) return;

			StopCoroutine(movementCoroutine);

			transform.SetParent(station.transform, true);

			transform.localPosition = Vector3.zero;

			//Debug.Log(transform.);

			embarkedSeat = seat;
		}

		public virtual void Disembark() {
			embarkedSeat.GetObject().GetComponent<TankStation>().Disembark();
			embarkedSeat = null;
			transform.position = transform.position + (Vector3.left * 2f);
			transform.SetParent(null);
		}*/

		public void OnDrawGizmos() {
			if (path != null) {
				for (int i = targetIndex; i < path.Length; i++) {
					Gizmos.color = Color.black;
					Gizmos.DrawCube(path[i], Vector3.one / 2);

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

		public void EnqueueCommand(Command command) {
			//CommManager.EnqueueCommand(command);
		}

		public void ExecuteCommand(Command command) {
			if (command != null) stateMachine.SubmitCommand(command);
			else Debug.Log("Null Command");
		}

		public void ExecuteCommand (string name) {

		}

		public GameObject GetObject() {
			return this.gameObject;
		}

		public ISelectable Select() {
			return this;
		}
	}
}