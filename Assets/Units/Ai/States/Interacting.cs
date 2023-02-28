using System.Collections;
using System.Collections.Generic;
using TankGame.Units.Interactions;
using UnityEngine;

namespace TankGame.Units.Ai {

	public class Interacting : TargetedState<AbstractInteraction> {

		public Interacting () { }

		public Interacting (AbstractInteraction interaction) {
			Target = interaction;
		}

		public override void Enter (Character actor) {
			base.Enter(actor);

			actor.SubmitTarget(Target.Parent.GetObject().transform, (success) => { });
		}

		public override void Act (Character actor) {
			if (Target.ActingCharacter.IntManager.IsInRange(Target.Parent.GetObject().transform)) {
				InteractionContext cntxt = actor.IntManager.Interact(Target);

				if (cntxt.Phase == IPhase.Post) End();
			}
		}
	}
}