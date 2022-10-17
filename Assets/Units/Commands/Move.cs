using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TankGame.Units.Pathfinding;

namespace TankGame.Units.Commands {

	public class Move : Command<Vector2> {

		public Move(Vector2 _targetPosition) : base(_targetPosition, "move") { }

		/*public override string Name() {
			return "MoveCommand";
		}*/

		public override void Start(Character character, Action<CommandContext> callback) {
			base.Start(character, callback);

			character.SubmitTarget(Target(), OnPathComplete);
		}

		public override void Cancel() {
			base.Cancel();

			Character.Stop();
		}

		private void OnPathComplete (bool success) {
			if (success) {
				Complete();
			}
			else {
				Cancel();
			}
		}
	}
}