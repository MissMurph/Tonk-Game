using System;
using System.Collections;
using System.Collections.Generic;
using TankGame.Units.Ai;
using TankGame.Units.Interactions;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TankGame.Units.Commands {

	public class Interact : TargetedCommand<Interaction> {

		public Interact(Interaction target) : base(Commands.GetTree("interact"), target) {
			
		}

		public override void Initialize (Character character) {
			//base.Initialize(character);

			Actor = character;

			Nodes[endNode].OnComplete += End;

			List<PreRequisite> transformReqs = Target.Parent.GetManager().GetPreRequisites();
			List<PreRequisite> interactionReqs = Target.Parent.GetPreRequisites();

			if (transformReqs is not null) PreRequisites.AddRange(transformReqs);
			if (interactionReqs is not null) PreRequisites.AddRange(interactionReqs);

			foreach (Decision node in Nodes) {
				node.Initialize(this, Target.Parent.GetObject().transform);

				if (node.CurrentState is TargetedState<Interaction>) {
					TargetedState<Interaction> state = node.CurrentState as TargetedState<Interaction>;
					state.SetTarget(Target);
				}
			}
		}
	}
}