using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TankGame.Units.Commands;
using UnityEngine;

namespace TankGame.Units.Ai {

	public class StateMachine : SerializedMonoBehaviour {

		[ReadOnly]
		[SerializeField]
		public Decision CurrentDecision { get; private set; } = null;

		private Queue<Decision> madeDecisions = new Queue<Decision>();

		[SerializeField]
		private float stateUpdateTime = 0.25f;

		[OdinSerialize]
		[DictionaryDrawerSettings(DisplayMode = DictionaryDisplayOptions.ExpandedFoldout)]
		private Dictionary<string, Goal> baseGoals = new Dictionary<string, Goal>();

		private Dictionary<string, Goal> newGoals = new Dictionary<string, Goal>();

		public Command CurrentCommand { get; private set; }

		private Queue<Command> commandQueue = new Queue<Command>();

		private Character character;

		private List<Decision> openSet = new List<Decision>();

		[ReadOnly]
		[SerializeField]
		private Dictionary<string, PreRequisite> preRequisites = new Dictionary<string, PreRequisite>();

		protected virtual void Awake () {
			character = GetComponent<Character>();

			foreach (Goal goal in baseGoals.Values) {
				goal.Initialize(character);
				openSet.AddRange(goal.GetStart());
			}

			CurrentDecision = openSet[0];
		}

		protected virtual void Start () {
			StartCoroutine(UpdateState());
		}

		private void Update () {
			if (CurrentCommand is null && commandQueue.TryDequeue(out Command command)) {
				openSet.AddRange(command.GetStart());
				command.OnComplete += CommandCallback;
				CurrentCommand = command;
			}
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
					if (!(CurrentDecision is null)) {
						CurrentDecision.Exit(character);
						if (!CurrentDecision.IsStart) openSet.Remove(CurrentDecision);
						CurrentDecision.Next.ForEach((decision) => openSet.Remove(decision));

						//CurrentDecision.Parent.GetStart().ForEach((decision) => { if (!openSet.Contains(decision)) openSet.Add(decision); });
					}

					if (!openSet.Contains(newState)) openSet.Add(newState);
					CurrentDecision = newState;
					openSet.AddRange(CurrentDecision.Next);

					List<PreRequisite> preReqs = new List<PreRequisite>();

					foreach (PreRequisite preReq in preRequisites.Values) {
						preReqs.Add(preReq);
					}

					CurrentDecision.Enter(character, preReqs.ToArray());
				}

				CurrentDecision.Act(character);

				ThreadStart newThread = delegate {
					MakeDecision(openSet, character.Stress, character.Morale, DecisionMade);
				};

				newThread.Invoke();
			}
		}

		private void DecisionMade(Decision nextState) {
			if (!ReferenceEquals(nextState, CurrentDecision)) lock (madeDecisions) {
				madeDecisions.Enqueue(nextState);
			}
		}

		//clears old result, sorts the openSet then makes a decision using random number generator and aggregated weights
		private void MakeDecision (List<Decision> decisions, int stress, int morale, Action<Decision> callback) {
			decisions.Sort();

			int middle = decisions.Count % 2 <= 0 ? decisions.Count / 2 : (decisions.Count + 1) / 2;
			int median = decisions[middle - 1].Weight;

			int[] modWeights = new int[decisions.Count];

			//Loop through each weight and amplify positively or negatively based on difference to Median
			for (int i = 0; i < decisions.Count; i++) {
				if (decisions[i].Weight <= median) {
					//negative amplification
					int newWeight = Mathf.RoundToInt(decisions[i].Weight - ((1 - (decisions[i].Weight / (float)median)) * (stress / 100f)));
					if (newWeight < 0) newWeight = 0;
					modWeights[i] = newWeight;
				}
				else {
					//Positive amplification
					int newWeight = Mathf.RoundToInt(decisions[i].Weight + ((1 - ((float)median / decisions[i].Weight)) * (morale / 100f)));
					if (newWeight < 0) newWeight = 0;
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

		public void ExecuteCommand (Command command) {
			commandQueue.Clear();

			if (!(CurrentCommand is null)) {
				if (ReferenceEquals(CurrentDecision.Parent, CurrentCommand)) {
					CurrentDecision.Next.ForEach((decision) => { openSet.Remove(decision); });
					openSet.Remove(CurrentDecision);
				}

				CurrentCommand.GetStart().ForEach((decision) => { openSet.Remove(decision); });

				CurrentCommand.OnComplete -= CommandCallback;
				CurrentCommand = null;
			}

			EnqueueCommand(command);
		}

		public void EnqueueCommand (Command command) {
			command.Initialize(character);
			commandQueue.Enqueue(command);
		}

		public void SubmitGoal (string name, Goal goal) {
			newGoals.Add(name, goal);
			goal.Initialize(character);
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

		public void SubmitPreRequisite (string key, IEvaluator condition, State solution) {
			PreRequisite output = new PreRequisite {
				condition = condition,
				solution = solution
			};

			preRequisites.Add(key, output);
		}

		public void ExpirePreRequisite (string key) {
			if (!preRequisites.ContainsKey(key)) return;

			preRequisites.Remove(key);
		}

		private void CommandCallback () {
			if (ReferenceEquals(CurrentDecision.Parent, CurrentCommand)) {
				foreach (Decision node in CurrentDecision.Next) {
					openSet.Remove(node);
				}
				openSet.Remove(CurrentDecision);
			}

			foreach (Decision node in CurrentCommand.GetStart()) {
				openSet.Remove(node);
			}

			CurrentCommand.OnComplete -= CommandCallback;
			CurrentCommand = null;
		}
	}
}