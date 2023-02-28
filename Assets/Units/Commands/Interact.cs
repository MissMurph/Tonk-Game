using System;
using System.Collections;
using System.Collections.Generic;
using TankGame.Units.Ai;
using TankGame.Units.Interactions;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TankGame.Units.Commands {

	public class Interact : TargetedCommand<AbstractInteraction> {

		public Interact(AbstractInteraction target) : base(Commands.GetTree("interact"), target) {
			
		}

		public override void Initialize (Character character) {
			//base.Initialize(character);

			Nodes[endNode].OnComplete += End;

			foreach (Decision node in Nodes) {
				node.Initialize(this, Target.Parent.GetObject().transform);

				if (node.CurrentState is TargetedState<AbstractInteraction>) {
					TargetedState<AbstractInteraction> state = node.CurrentState as TargetedState<AbstractInteraction>;
					state.SetTarget(Target);
				}
			}
		}
	}
}