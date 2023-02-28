using System;
using System.Collections;
using System.Collections.Generic;
using TankGame.Units.Ai;
using TankGame.Units.Navigation;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TankGame.Units.Commands {

	public class Move : TargetedCommand<Vector2> {

		private Character character;

		public Move(Vector2 _targetPosition) : base(Commands.GetTree("move"), _targetPosition) {
			
		}

		public override void Initialize (Character character) {
			//base.Initialize(character);

			this.character = character;

			Nodes[endNode].OnComplete += End;

			Transform convertedPos = character.targetTracker;

			convertedPos.position = Target;

			convertedPos.SetParent(World.GlobalTraversable.GetObject().transform);

			foreach (Decision node in Nodes) {
				node.Initialize(this, convertedPos);

				if (node.CurrentState is TargetedState<Vector2>) {
					TargetedState<Vector2> state = node.CurrentState as TargetedState<Vector2>;
					state.SetTarget(Target);
				}
			}
		}

		protected override void End () {
			character.targetTracker.SetParent(character.transform);
		}
	}
}