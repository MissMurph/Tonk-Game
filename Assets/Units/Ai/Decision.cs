using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Sirenix.OdinInspector;

namespace TankGame.Units.Ai {
	
	[Serializable]
	public class Decision : IComparable {

		[SerializeField] public string Name { get; private set; }
		[SerializeField] public State State { get; private set; }

		//Base Weight is not modified at all during run-time, any modifications are applied to ModWeight so the same Modification can be reversed. Query Weight for the combined Base + Mod Weight
		[SerializeField] public int BaseWeight { get; private set; }

		public bool IsStart {
			get {
				return Parent.GetStart().Contains(this);
			}
		}

		private Goal Parent { get; set; }

		[ReadOnly]
		[SerializeField]
		public int ModWeight {
			get {
				int weight = 0;

				foreach (KeyValuePair<int, WeightBehaviour> entry in evalWeights) {
					if (Evaluators[entry.Key].Act()) {
						weight += entry.Value.value;
					}
				}

				return weight;
			}
		}

		[SerializeField] public int Weight { 
			get { 
				return 
					BaseWeight + ModWeight + Parent.Weight; 
			} 
		}

		//All nodes have a position in Goal's array. Enter the array index of the next nodes to link
		[SerializeField] private List<int> nextNodes = new List<int>();

		public List<Decision> Next { get; private set; } = new List<Decision>();

		[SerializeField] public List<IEvaluator> Evaluators { get; private set; } = new List<IEvaluator>();
		[SerializeField] private Dictionary<int, WeightBehaviour> evalWeights = new Dictionary<int, WeightBehaviour>();

		public Decision () {

		}

		//copy constructor
		public Decision (Decision decision) {
			State = decision.State;
			BaseWeight = decision.BaseWeight;
			nextNodes.AddRange(decision.nextNodes);
			Evaluators.AddRange(decision.Evaluators);
			evalWeights = decision.evalWeights;
		}

		public void Initialize (Goal parentGoal) {
			Parent = parentGoal;

			foreach (int index in nextNodes) {
				Next.Add(Parent.Nodes[index]);
			}
		}

		public int CompareTo (object obj) {
			Decision decision = obj as Decision;

			if (decision != null) {
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
}