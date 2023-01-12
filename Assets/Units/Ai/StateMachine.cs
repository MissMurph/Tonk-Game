using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace TankGame.Units.Ai {

	public class StateMachine : SerializedMonoBehaviour {

		public Decision State { get; private set; }

		private float stateUpdateTime = 0.25f;  //Four updates per second

		[SerializeField]
		[DictionaryDrawerSettings(DisplayMode = DictionaryDisplayOptions.ExpandedFoldout)]
		private Dictionary<string, Goal> baseGoals = new Dictionary<string, Goal>();

		private List<Decision> availableDecisions = new List<Decision>();

		private Character character;

		//[SerializeField]
		//private GoalEntry[] entries;

		protected virtual void Awake () {
			character = GetComponent<Character>();

			foreach (KeyValuePair<string, Goal> entry in baseGoals) {
				foreach (Decision node in entry.Value.GetStart()) {
					availableDecisions.Add(node);
				}
			}
		}

		protected virtual void Start () {
			StartCoroutine(UpdateState());
		}

		IEnumerator UpdateState () {
			if (Time.timeSinceLevelLoad < 1f) {
				yield return new WaitForSeconds(0.2f);
			}

			while (true) {
				yield return new WaitForSeconds(stateUpdateTime);

				Decision oldState = State;

				Decision nextState = WeightedRandom();

				if (ReferenceEquals(nextState, State)) State = nextState;

				State.State.Act(character);

				/*if (!ExecutingCommand && commandQueue.TryDequeue(out Command command)) {
					StopCoroutine(currentCoroutine);
					PerformCommand(command);
				}
				else if (ActiveCommand != null) {
					ActiveCommand.Perform();
				}
				*/
			}
		}

		private Decision WeightedRandom () {
			int[] weights = new int[availableDecisions.Count];
			int totalWeight = 0;

			for (int i = 0; i < availableDecisions.Count; i++) {
				weights[i] = availableDecisions[i].Weight;
				totalWeight += weights[i];
			}

			System.Random rando = new System.Random();

			int result = rando.Next(0, totalWeight);

			for (int i = availableDecisions.Count - 1; i > 0; i--) {
				totalWeight -= weights[i];

				if (result >= totalWeight) return availableDecisions[i];
			}

			return null;
		}
	}
}