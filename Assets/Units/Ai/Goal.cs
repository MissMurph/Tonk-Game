using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankGame.Units.Ai {

	public class Goal {

		public string Name { get; private set; }

		public int BaseWeight { get; private set; }
		public int Weight { get; private set; }

		private class Decision {

			public State State { get; private set; }

			public int BaseWeight { get; private set; }
			public int Weight { get; private set; }

			public List<State> Next { get; private set; }

			public Decision (State _state, int _baseWeight) {
				State = _state;
				BaseWeight = _baseWeight;
			}
		}
	}
}