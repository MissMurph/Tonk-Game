using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

namespace TankGame.Units.Ai {

	[Serializable]
	public class Goal {

		//[SerializeField] public string Name { get; private set; }

		[SerializeField] public int BaseWeight { get; private set; } = 100;
		[ReadOnly] [SerializeField] public int ModWeight { get; private set; }
		[SerializeField] public int Weight { get { return BaseWeight + ModWeight; } }

		[OdinSerialize]
		//[ValueDropdown("nodes")]
		protected List<int> startNodes;

		[OdinSerialize]
		public Decision[] Nodes { get; protected set; }

		public Character Actor { get; protected set; }

		public List<PreRequisite> PreRequisites { get; protected set; }

		/*public Goal (string _name, int _baseWeight, Decision[] _nodes, int[] _startNodes) {
			Name = _name;
			BaseWeight = _baseWeight;
			nodes = _nodes;
			startNodes = _startNodes;
		}*/

		public Goal () {
			PreRequisites = new List<PreRequisite>();
			startNodes = new List<int>();
		}

		protected Goal (Goal goal) {
			PreRequisites = new List<PreRequisite>();
			startNodes = new List<int>();

			BaseWeight = goal.BaseWeight;

			startNodes.AddRange(goal.startNodes);

			Nodes = new Decision[goal.Nodes.Length];

			for (int i = 0; i < goal.Nodes.Length; i++) {
				Nodes[i] = new Decision(goal.Nodes[i]);
			}
		}

		public virtual void Initialize (Character character) {
			Actor = character;

			foreach (Decision node in Nodes) {
				node.Initialize(this);
			}
		}

		public List<Decision> GetStart () {
			List<Decision> output = new List<Decision>();
			
			foreach (int index in startNodes) {
				output.Add(Nodes[index]);
			}

			return output;
		}
	}
}