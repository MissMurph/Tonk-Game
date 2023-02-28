using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

namespace TankGame.Units.Ai {
	
	[Serializable]
	public class Decision : IComparable {

		[SerializeField] public string Name { get; private set; }

		[ReadOnly][SerializeField] public State State { get { return currentState; } }

		private State currentState;

		[SerializeField] private State baseState;

		//Base Weight is not modified at all during run-time, any modifications are applied to ModWeight so the same Modification can be reversed. Query Weight for the combined Base + Mod Weight
		[SerializeField] public int BaseWeight { get; private set; }

		public bool IsStart {
			get {
				return Parent.GetStart().Contains(this);
			}
		}

		public Goal Parent { get; private set; }

		[ReadOnly]
		[SerializeField]
		public int ModWeight {
			get {
				int weight = 0;

				foreach (KeyValuePair<int, WeightBehaviour> entry in evalWeights) {
					if (Evaluators[entry.Key].Act(Parent.Actor)) {
						weight += entry.Value.value;
					}
				}

				return weight;
			}
		}

		[SerializeField] public int Weight { 
			get { 
				return Mathf.Max(0, BaseWeight + ModWeight + Parent.Weight);
			} 
		}

		public delegate void OnEnd();

		public event OnEnd OnComplete;

		public Transform Target { get; private set; }

		//All nodes have a position in Goal's array. Enter the array index of the next nodes to link
		[SerializeField] private List<int> nextNodes = new List<int>();

		public List<Decision> Next { get; private set; } = new List<Decision>();

		[SerializeField] public List<IEvaluator> Evaluators { get; private set; } = new List<IEvaluator>();
		[SerializeField] private Dictionary<int, WeightBehaviour> evalWeights = new Dictionary<int, WeightBehaviour>();

		[SerializeField] private PreRequisite[] preRequisites;

		private List<PreRequisite> addedPreRequisites = new List<PreRequisite>();

		private Queue<PreRequisite> preRequisuiteFulfillmentQueue = new Queue<PreRequisite>();

		public Decision () {
			currentState = baseState;
		}

		//copy constructor
		public Decision (Decision decision) {
			baseState = decision.State;
			currentState = baseState;
			BaseWeight = decision.BaseWeight;
			nextNodes.AddRange(decision.nextNodes);
			Evaluators.AddRange(decision.Evaluators);
			evalWeights = decision.evalWeights;
		}

		//This will check each Pre-Requisite and add them to a queue to be completed. This check will re-run itself whenever the current state is complete.
		public void Enter (Character character) {
			baseState.OnComplete += () => { if (OnComplete != null) OnComplete.Invoke(); };

			foreach (PreRequisite preReq in addedPreRequisites) {
				if (preReq.condition.Act(Parent.Actor)) preRequisuiteFulfillmentQueue.Enqueue(preReq);
			}

			foreach (PreRequisite preReq in preRequisites) {
				if (preReq.condition.Act(Parent.Actor)) preRequisuiteFulfillmentQueue.Enqueue(preReq);
			}

			if (preRequisuiteFulfillmentQueue.TryDequeue(out PreRequisite requirement)) {
				currentState = requirement.solution;
				currentState.OnComplete += () => { Enter(character); };
			}
			else {
				currentState = baseState;
			}

			State.Enter(character);
		}

		public void Enter (Character character, params PreRequisite[] preRequisites) {
			addedPreRequisites.AddRange(preRequisites);

			Enter(character);
		}

		public void Exit (Character character) {
			addedPreRequisites.Clear();

			State.Exit(character);
		}

		public void Act (Character character) {
			State.Act(character);
		}

		public void Initialize (Goal parentGoal) {
			Parent = parentGoal;

			foreach (int index in nextNodes) {
				Next.Add(Parent.Nodes[index]);
			}
		}

		public void Initialize (Goal parentGoal, Transform target) {
			Target = target;

			Initialize(parentGoal);
		}

		public int CompareTo (object obj) {
			if (obj is Decision decision) {
				return Weight.CompareTo(decision.Weight);
			}
			else {
				throw new ArgumentException("object is not a decision!");
			}
		}

		[Serializable]
		public class WeightBehaviour {
			public int value;
		}
	}

	[Serializable]
	public class PreRequisite {
		public IEvaluator condition;
		public State solution;
	}
}