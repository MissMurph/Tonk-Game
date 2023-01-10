using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankGame.Units.Ai {

	public class Goal {

		public string Name { get; private set; }

		public int BaseWeight { get; private set; }
		public int Weight { get; private set; }

		private int[] startNodes;
		//private List<Decision> nodes = new List<Decision>();

		private Decision[] nodes;

		public Goal (string _name, int _baseWeight, Decision[] _nodes, int[] _startNodes) {
			Name = _name;
			BaseWeight = _baseWeight;
			nodes = _nodes;
			startNodes = _startNodes;
		}

		
	}
}