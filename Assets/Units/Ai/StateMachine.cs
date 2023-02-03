using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Jobs;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using TankGame.Units.Commands;
using Unity.Collections;
using System;

namespace TankGame.Units.Ai {

	public class StateMachine : SerializedMonoBehaviour {

		[Sirenix.OdinInspector.ReadOnly]
		[SerializeField]
		public Decision State { get; private set; }

		[SerializeField]
		private float stateUpdateTime = 0.25f;  //Four updates per second

		[OdinSerialize]
		[DictionaryDrawerSettings(DisplayMode = DictionaryDisplayOptions.ExpandedFoldout)]
		private Dictionary<string, Goal> baseGoals = new Dictionary<string, Goal>();

		private Dictionary<string, Goal> newGoals = new Dictionary<string, Goal>();

		private Command currentCommand;

		private Character character;

		private List<Decision> openSet = new List<Decision>();

		private static System.Random tempRando = new System.Random();

		private NativeArray<int> decisionResult;

		private JobHandle currentJob;

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

			MakeDecision();

			while (true) {
				yield return new WaitForSeconds(stateUpdateTime);

				currentJob.Complete();

				Decision oldState = State;
				
				Decision nextState = openSet[decisionResult[0]];

				if (!ReferenceEquals(nextState, State)) {
					State = nextState;
					if (oldState != null) oldState.State.Exit(character);
					State.State.Enter(character);
				}
				
				State.State.Act(character);

				MakeDecision();
			}
		}

		//clears old result, sorts the openSet then makes a decision using random number generator and aggregated weights
		private void MakeDecision () {
			//decisionResult.Dispose();
			
			decisionResult = new NativeArray<int>(1, Allocator.Temp);

			//openSet.Sort();

			Decisionlet[] decisionWeights = new Decisionlet[openSet.Count];

			for (int i = 0; i < openSet.Count; i++) {
				decisionWeights[i] = new Decisionlet(i, openSet[i].Weight);
			}

			currentJob = new DecisionJob(decisionWeights, character.Stress, character.Morale, decisionResult).Schedule();
		}

		public void SubmitCommand (Command command) {
			if (currentCommand != null) currentCommand.OnComplete -= CommandCallback;
			currentCommand = command;
			currentCommand.Initialize();
			currentCommand.OnComplete += CommandCallback;
		}

		public void SubmitGoal (Goal goal) {

		}

		public void ExpireGoal () {

		}

		private void CommandCallback () {
			currentCommand = null;
		}

		public struct Decisionlet : IComparable<Decisionlet> {
			public int Index;
			public int Weight;

			public Decisionlet (int _index, int _weight) {
				Index = _index;
				Weight = _weight;
			}

			public int CompareTo (Decisionlet other) {
				return Weight - other.Weight;
			}

			public void SetWeight (int newWeight) {
				Weight = newWeight;
			}
		}

		//In theory could have a generic version of this (for the output) and create a universal job that takes a Delegate to execute? Will research for Thread Pooling and if we have other Job uses
		private struct DecisionJob : IJob {

			private NativeList<Decisionlet> decisions;

			private int stress;
			private int morale;

			private NativeArray<int> result;

			//Cant accept decisions as all data needs to be collected from the game before Job can run, therefore Weight Collection must occur before.
			//Decisions come in pre sorted according to weight, simply have to calculate median, apply magnitude to weights and select a random weight
			public DecisionJob (Decisionlet[] _weights, int _stress, int _morale, NativeArray<int> _result) {
				stress = _stress;
				morale = _morale;
				result = _result;

				decisions = new NativeList<Decisionlet>(_weights.Length, Allocator.TempJob);

				for (int i = 0; i < _weights.Length; i++) {
					decisions.Add(_weights[i]);
				}
			}

			public void Execute () {
				decisions.Sort();

				int middle = decisions.Length % 2 <= 0 ? decisions.Length / 2 : (decisions.Length + 1) / 2;
				int median = decisions[middle].Weight;

				//Loop through each weight and amplify positively or negatively based on difference to Median
				for (int i = 0; i < decisions.Length; i++) {
					if (decisions[i].Weight <= median) {
						//negative amplification
						int newWeight = Mathf.RoundToInt(decisions[i].Weight - ((1 - (decisions[i].Weight / (float)median)) * (stress / 100f)));
						decisions[i].SetWeight(newWeight);
					}
					else {
						//Positive amplification
						int newWeight = Mathf.RoundToInt(decisions[i].Weight + ((1 - ((float)median / decisions[i].Weight)) * (morale / 100f)));
						decisions[i].SetWeight(newWeight);
					}
				}

				//Add up all weights into one total
				int totalWeight = 0;

				for (int i = 0; i < decisions.Length; i++) {
					totalWeight += decisions[i].Weight;
				}

				//Choose a random point in that total
				System.Random rando = new System.Random();

				int randomNumber = rando.Next(0, totalWeight);

				//Loop backwards over the collection (as number is much more likely to land in larger weights), subtract the weight from the total then compare the random number with the new total
				//If the random number is greater than the total, we've just subtracted the weight it's landed in
				for (int i = decisions.Length - 1; i >= 0; i--) {
					//totalWeight -= decisions[i];
					
					if (randomNumber >= totalWeight) {
						result[0] = decisions[i].Index;
						decisions.Dispose();
						return;
					}
				}
			}
		}
	}
}