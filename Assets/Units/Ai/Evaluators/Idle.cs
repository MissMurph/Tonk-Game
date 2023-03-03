using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankGame.Units.Ai {

	public class Idle : IEvaluator {

		public Idle() {

		}

		public bool Act(Character character) {
			return character.StateMachine.CurrentCommand is null;
		}

		public string Name() {
			return "character_idle";
		}
	}
}