using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

namespace TankGame.Units.Ai {
	
	[Serializable]
	public class Decision : IComparable {

		[ReadOnly]
		[SerializeField]
		public State CurrentState { get; private set; }

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

		[SerializeField]
		public int Weight { 
			get { 
				return Mathf.Max(0, BaseWeight + ModWeight + Parent.Weight);
			} 
		}

		public delegate void OnEnd();

		public event OnEnd OnComplete;

		public Transform Target { get; private set; }

		public List<Decision> Next { get; private set; }


		/*	Inspector Fields	*/
		/*	These Fields are initialized typically in Inspector, need to verify existence before using	*/

		[SerializeField]
		public string Name { get; private set; }

		[SerializeField]
		public int BaseWeight { get; private set; }

		[SerializeField]
		private State baseState;

		[SerializeField]
		private List<int> nextNodes = new List<int>();

		[SerializeField] 
		public List<IEvaluator> Evaluators { get; private set; } = new List<IEvaluator>();

		[SerializeField] 
		private Dictionary<int, WeightBehaviour> evalWeights = new Dictionary<int, WeightBehaviour>();


		/*	Pre-Requisites	*/
		/*	Pre-Requisites are stored in the below list, when one needs to be fulfilled it will be added to the Queue, so that each Pre-Req. can be fulfilled	*/
		private List<PreRequisite> preRequisites;

		private Queue<PreRequisite> preRequisuiteFulfillmentQueue;

		//Odin Constructor
		public Decision () {}

		//copy constructor
		public Decision (Decision decision) {
			baseState = decision.CurrentState;
			BaseWeight = decision.BaseWeight;
			nextNodes.AddRange(decision.nextNodes);
			Evaluators.AddRange(decision.Evaluators);
			evalWeights = decision.evalWeights;
		}

		//This will check each Pre-Requisite and add them to a queue to be completed. This check will re-run itself whenever the current state is complete.
		public void Enter (Character character) {
			baseState.OnComplete += () => { if (OnComplete != null) OnComplete.Invoke(); };

			foreach (PreRequisite preReq in preRequisites) {
				if (preReq.condition.Act(Parent.Actor)) preRequisuiteFulfillmentQueue.Enqueue(preReq);
			}

			if (preRequisuiteFulfillmentQueue.TryDequeue(out PreRequisite requirement)) {
				CurrentState = requirement.solution;
				CurrentState.OnComplete += () => { Enter(character); };
			}
			else {
				CurrentState = baseState;
			}

			CurrentState.Enter(character);
		}

		public void Enter (Character character, params PreRequisite[] preRequisites) {
			this.preRequisites.AddRange(preRequisites);

			Enter(character);
		}

		public void Exit (Character character) {
			preRequisites.Clear();

			CurrentState.Exit(character);
		}

		public void Act (Character character) {
			CurrentState.Act(character);
		}

		public void Initialize (Goal parentGoal) {
			Parent = parentGoal;
			CurrentState = baseState;

			Next = new List<Decision>();

			foreach (int index in nextNodes) {
				Next.Add(Parent.Nodes[index]);
			}

			preRequisites = new List<PreRequisite>();
			preRequisuiteFulfillmentQueue = new Queue<PreRequisite>();
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