using System.Collections;
using System.Collections.Generic;
using TankGame.Units.Commands;
using TankGame.Units.Navigation;
using UnityEngine;

namespace TankGame.Units.Ai {

	public class NeedsToEmbark : IEvaluator {

		private ITraversable parent;
		private Transform target;

		public NeedsToEmbark (ITraversable _target) {
			parent = _target;
		}

		public bool Act (Character character) {
			if (ReferenceEquals(target.transform.parent, parent.GetObject().transform) && !ReferenceEquals(character.Traversable, parent)) {
				return true;
			}

			return false;
		}

		public string Name () {
			return "character_needs_to_embark";
		}

		public void DecisionInjector(Decision decision) {
			target = decision.Target;
		}
	}
}