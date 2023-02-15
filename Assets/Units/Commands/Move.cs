using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TankGame.Units.Commands {

	public class Move : TargetedCommand<Vector2> {

		public Move(Vector2 _targetPosition) : base(Commands.GetTree("move"), _targetPosition) {
			
		}
	}
}