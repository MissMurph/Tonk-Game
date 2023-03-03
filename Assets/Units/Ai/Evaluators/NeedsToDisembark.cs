using System.Collections;
using System.Collections.Generic;
using TankGame.Units.Commands;
using TankGame.Units.Navigation;
using UnityEngine;

namespace TankGame.Units.Ai {

	public class NeedsToDisembark : IEvaluator {

		private ITraversable parent;
		private Transform target;

		public NeedsToDisembark(ITraversable _target) {
			parent = _target;
		}

		public bool Act (Character character) {
			if (target is not null && target.parent.TryGetComponent(out ITraversable targetTrav)) {
				if (!ReferenceEquals(parent, targetTrav) && ReferenceEquals(parent, character.Traversable)) {
					return true;
				}
			}

			return false;
		}

		public string Name () {
			return "character_needs_to_disembark";
		}

		public void DecisionInjector (Decision decision) {
			target = decision.Target;
		}
	}
}