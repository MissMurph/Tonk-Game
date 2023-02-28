using System.Collections;
using System.Collections.Generic;
using TankGame.Units.Commands;
using TankGame.Units.Navigation;
using UnityEngine;

namespace TankGame.Units.Ai {

	public class Embarkment : IEvaluator {

		private ITraversable parent;
		private Transform target;

		public Embarkment (ITraversable _target) {
			parent = _target;
		}

		public bool Act (Character character) {
			return target.parent.TryGetComponent(out ITraversable traversable) && !ReferenceEquals(parent, traversable);
		}

		public string Name () {
			return "character_embarkment";
		}

		public void DecisionInjector (Decision decision) {
			target = decision.Target;
		}
	}
}