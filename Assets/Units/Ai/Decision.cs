using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Sirenix.OdinInspector;

namespace TankGame.Units.Ai {

	[Serializable]
	public class Decision {

		[SerializeField] public string Name { get; private set; }
		[SerializeField] public State State { get; private set; }

		//Base Weight is not modified at all during run-time, any modifications are applied to ModWeight so the same Modification can be reversed. Query Weight for the combined Base + Mod Weight
		[SerializeField] public int BaseWeight { get; private set; }

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

		[SerializeField] public int Weight { get { return BaseWeight + ModWeight; } }

		//All nodes have a position in Goal's array. Enter the array index of the next nodes to link
		[SerializeField] public List<int> Next { get; private set; } = new List<int>();
		[SerializeField] public List<IEvaluator> Evaluators { get; private set; } = new List<IEvaluator>();
		[SerializeField] private Dictionary<int, WeightBehaviour> evalWeights = new Dictionary<int, WeightBehaviour>();

		public Decision () {

		}

		[Serializable]
		public class WeightBehaviour {
			public int value;
		}
	}
}