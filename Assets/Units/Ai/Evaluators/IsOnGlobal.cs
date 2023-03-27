using System.Collections;
using System.Collections.Generic;
using TankGame.Units.Navigation;
using UnityEngine;

namespace TankGame.Units.Ai {

	public class IsOnGlobal : IEvaluator {
		
		public bool Act (Character character) {
			return ReferenceEquals(character.Traversable, World.GlobalTraversable);
		}

		public string Name () {
			return "is_on_global_traversable";
		}
	}
}