using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Jobs;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using TankGame.Units.Commands;
using Unity.Collections;
using System;
using System.Threading;

namespace TankGame.Units.Ai {

	public class StateMachine : SerializedMonoBehaviour {

		[Sirenix.OdinInspector.ReadOnly]
		[SerializeField]
		public Decision State { get; private set; } = null;

		private Queue<Decision> madeDecisions = new Queue<Decision>();

		[SerializeField]
		private float stateUpdateTime = 0.25f;

		[OdinSerialize]
		[DictionaryDrawerSettings(DisplayMode = DictionaryDisplayOptions.ExpandedFoldout)]
		private Dictionary<string, Goal> baseGoals = new Dictionary<string, Goal>();

		private Dictionary<string, Goal> newGoals = new Dictionary<string, Goal>();

		private Command currentCommand;

		private Character character;

		private List<Decision> openSet = new List<Decision>();

		private static System.Random tempRando = new System.Random();

		protected virtual void Awake () {
			character = GetComponent<Character>();

			foreach (Goal goal in baseGoals.Values) {
				goal.Initialize();
				openSet.AddRange(goal.GetStart());
			}
		}

		protected virtual void Start () {
			StartCoroutine(UpdateState());
		}

		IEnumerator UpdateState () {
			if (Time.timeSinceLevelLoad < 1f) {
				yield return new WaitForSeconds(0.2f);
			}

			ThreadStart starterThread = delegate {
				MakeDecision(openSet, character.Stress, character.Morale, DecisionMade);
			};

			starterThread.Invoke();

			while (true) {
				yield return new WaitForSeconds(stateUpdateTime);

				if (madeDecisions.TryDequeue(out Decision newState)) {
					if (State is null) State.State.Exit(character);

					State = newState;
					State.State.Enter(character);
				}

				State.State.Act(character);

				ThreadStart newThread = delegate {
					MakeDecision(openSet, character.Stress, character.Morale, DecisionMade);
				};

				newThread.Invoke();
			}
		}

		private void DecisionMade(Decision nextState) {
			if (!ReferenceEquals(nextState, State)) lock (madeDecisions) {
				madeDecisions.Enqueue(nextState);
			}
		}

		//clears old result, sorts the openSet then makes a decision using random number generator and aggregated weights
		private void MakeDecision (List<Decision> decisions, int stress, int morale, Action<Decision> callback) {
			decisions.Sort();

			int middle = decisions.Count % 2 <= 0 ? decisions.Count / 2 : (decisions.Count + 1) / 2;
			int median = decisions[middle].Weight;

			int[] modWeights = new int[decisions.Count];

			//Loop through each weight and amplify positively or negatively based on difference to Median
			for (int i = 0; i < decisions.Count; i++) {
				if (decisions[i].Weight <= median) {
					//negative amplification
					int newWeight = Mathf.RoundToInt(decisions[i].Weight - ((1 - (decisions[i].Weight / (float)median)) * (stress / 100f)));
					modWeights[i] = newWeight;
				}
				else {
					//Positive amplification
					int newWeight = Mathf.RoundToInt(decisions[i].Weight + ((1 - ((float)median / decisions[i].Weight)) * (morale / 100f)));
					modWeights[i] = newWeight;
				}
			}

			//Add up all weights into one total
			int totalWeight = 0;

			for (int i = 0; i < decisions.Count; i++) {
				totalWeight += modWeights[i];
			}

			//Choose a random point in that total
			System.Random rando = new System.Random();

			int randomNumber = rando.Next(0, totalWeight);

			//Loop backwards over the collection (as number is much more likely to land in larger weights), subtract the weight from the total then compare the random number with the new total
			//If the random number is greater than the total, we've just subtracted the weight it's landed in
			for (int i = decisions.Count - 1; i >= 0; i--) {
				totalWeight -= modWeights[i];

				if (randomNumber >= totalWeight) {
					callback.Invoke(decisions[i]);
					return;
				}
			}
		}

		public void SubmitCommand (Command command) {
			if (currentCommand != null) currentCommand.OnComplete -= CommandCallback;
			currentCommand = command;
			currentCommand.Initialize();
			openSet.AddRange(currentCommand.GetStart());
			currentCommand.OnComplete += CommandCallback;
		}

		public void SubmitGoal (string name, Goal goal) {
			newGoals.Add(name, goal);
			openSet.AddRange(goal.GetStart());
		}

		public void ExpireGoal (string name) {
			if (newGoals.TryGetValue(name, out Goal goal)) {
				foreach (Decision decision in goal.GetStart()) {
					openSet.Remove(decision);
				}

				newGoals.Remove(name);
			}
		}

		private void CommandCallback () {
			if (ReferenceEquals(State.Parent, currentCommand)) {
				foreach (Decision node in State.Next) {
					openSet.Remove(node);
				}
			}

			foreach (Decision node in currentCommand.GetStart()) {
				openSet.Remove(node);
			}

			currentCommand = null;
		}
	}
}