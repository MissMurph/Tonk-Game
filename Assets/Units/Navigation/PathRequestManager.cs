using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using System.Threading.Tasks;

namespace TankGame.Units.Navigation {

	public class PathRequestManager : MonoBehaviour {

		private static PathRequestManager instance;

		private Queue<PathResult> results = new Queue<PathResult>();

		private void Awake() {
			instance = this;
		}

		private void Update() {
			if (results.Count > 0) {
				int itemsInQueue = results.Count;

				lock (results) {
					for (int i = 0; i < itemsInQueue; i++) {
						PathResult result = results.Dequeue();
						result.callback(result.path, result.success);
					}
				}
			}
		}

		/*public static void RequestPath(Vector3 startPos, Vector3 targetPos, ITraversable traversable, Action<Vector3[], bool> callback) {
			ThreadStart threadStart = delegate {
				traversable.FindPath(new PathRequest(startPos, targetPos, traversable, callback), instance.FinishedProcessingPath);
			};

			threadStart.Invoke();
		}*/

		public static void RequestPath (Transform origin, Transform target, ITraversable traversable, Action<Vector3[], bool> callback) {
			ThreadStart threadStart = delegate {
				traversable.FindPath(new PathRequest(origin, target, traversable, callback), instance.FinishedProcessingPath);
			};

			threadStart.Invoke();
		}

		public void FinishedProcessingPath(PathResult result) {
			lock (results) {
				results.Enqueue(result);
			}
		}

		private void OnDestroy () {
			instance = null;
		}
	}
	  
	public struct PathResult {
		public Vector3[] path;
		public bool success;
		public Action<Vector3[], bool> callback;

		public PathResult(Vector3[] _path, bool _success, Action<Vector3[], bool> _callback) {
			path = _path;
			success = _success;
			callback = _callback;
		}
	}

	public struct PathRequest {
		public Transform pathStart;
		public Transform pathEnd;

		public ITraversable originTraversable;
		public ITraversable targetTraversable;

		public Action<Vector3[], bool> callback;

		public PathRequest (Transform _origin, Transform _target, ITraversable _traversable, Action<Vector3[], bool> _callback) {
			pathStart = _origin;
			pathEnd = _target;

			originTraversable = _traversable;
			targetTraversable = _target.GetComponentInParent<ITraversable>();

			callback = _callback;
		}
	}
}