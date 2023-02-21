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
			return ReferenceEquals(character.Traversable, parent);
		}

		public string Name () {
			return "character_embarkment";
		}

		public void CommandInjector (Command command) {
			
		}
	}
}