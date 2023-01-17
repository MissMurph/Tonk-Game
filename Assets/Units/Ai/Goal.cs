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

		[SerializeField] public int BaseWeight { get; private set; }
		[ReadOnly] [SerializeField] public int ModWeight { get; private set; }
		[SerializeField] public int Weight { get { return BaseWeight + ModWeight; } }

		[OdinSerialize]
		//[ValueDropdown("nodes")]
		private List<int> startNodes = new List<int>();

		[OdinSerialize]
		public Decision[] Nodes { get; private set; }

		/*public Goal (string _name, int _baseWeight, Decision[] _nodes, int[] _startNodes) {
			Name = _name;
			BaseWeight = _baseWeight;
			nodes = _nodes;
			startNodes = _startNodes;
		}*/

		public Goal () {
			
		}

		public void Initialize () {
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