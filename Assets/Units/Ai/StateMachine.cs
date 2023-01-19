using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using TankGame.Units.Commands;

namespace TankGame.Units.Ai {

	public class StateMachine : SerializedMonoBehaviour {

		//[ReadOnly]
		//[SerializeField]
		public Decision State { get; private set; }

		[SerializeField]
		private float stateUpdateTime = 0.25f;  //Four updates per second

		[OdinSerialize]
		[DictionaryDrawerSettings(DisplayMode = DictionaryDisplayOptions.ExpandedFoldout)]
		private Dictionary<string, Goal> baseGoals = new Dictionary<string, Goal>();

		private Command currentCommand;

		private Character character;

		//[SerializeField]
		//private GoalEntry[] entries;

		protected virtual void Awake () {
			character = GetComponent<Character>();

			foreach (Goal goal in baseGoals.Values) {
				goal.Initialize();
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

				if (!ReferenceEquals(nextState, State)) {
					State = nextState;
					if (oldState != null) oldState.State.Exit(character);
					State.State.Enter(character);
				}
				
				State.State.Act(character);
			}
		}

		public void SubmitCommand (Command command) {
			if (currentCommand != null) currentCommand.OnComplete -= CommandCallback;
			currentCommand = command;
			currentCommand.Initialize();
			currentCommand.OnComplete += CommandCallback;
		}

		private void CommandCallback () {
			currentCommand = null;
		}

		private List<Decision> GetAvailable () {
			List<Decision> output = new List<Decision>();

			if (State != null) {
				if (!State.State.Complete) output.Add(State);
				if (State.Next != null) output.AddRange(State.Next);
			}

			if (currentCommand != null) {
				foreach (Decision node in currentCommand.GetStart()) {
					if (!ReferenceEquals(node, State)) {
						output.Add(node);
					}
				}
			}

			foreach (Goal goal in baseGoals.Values) {
				foreach (Decision node in goal.GetStart()) {
					if (!ReferenceEquals(node, State)) {
						output.Add(node);
					}
				}
			}

			return output;
		}

		private Decision WeightedRandom () {
			List<Decision> availableDecisions = GetAvailable();

			int[] weights = new int[availableDecisions.Count];
			int totalWeight = 0;

			for (int i = 0; i < availableDecisions.Count; i++) {
				weights[i] = availableDecisions[i].Weight;
				totalWeight += weights[i];
			}

			System.Random rando = new System.Random();

			int result = rando.Next(0, totalWeight);

			for (int i = availableDecisions.Count - 1; i >= 0; i--) {
				totalWeight -= weights[i];

				if (result >= totalWeight) return availableDecisions[i];
			}

			return State;
		}
	}
}