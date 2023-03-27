using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankGame.Units.Ai {

	public class Moving : TargetedState<Vector2> {

		public override void Enter (Character actor) {
			actor.SubmitTarget(Target, Callback);
		}

		private void Callback (bool success) {
			End();
		}

		public override void Act (Character actor) {
			
		}
	}
}