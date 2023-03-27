using System.Collections;
using System.Collections.Generic;
using TankGame.Units.Interactions;
using UnityEngine;

namespace TankGame.Units.Ai {

	public class Interacting : TargetedState<AbstractInteraction> {

		public delegate AbstractInteraction Supplier(Character character, string name);

		private Supplier supplier;
		private string name;

		public Interacting () { }

		public Interacting (AbstractInteraction interaction) {
			Target = interaction;
		}

		public Interacting(Supplier _supplier, string _name) {
			//Target = interaction;
			supplier = _supplier;
			name = _name;
		}

		public override void Enter (Character actor) {
			base.Enter(actor);

			if (supplier is not null) {
				Target = supplier.Invoke(actor, name);
			}

			actor.SubmitTarget(Target.Parent.GetObject().transform, (success) => { });
		}

		public override void Act (Character actor) {
			if (Target.ActingCharacter.IntManager.IsInRange(Target.Parent.GetObject().transform)) {
				InteractionContext cntxt = Target.ActingCharacter.IntManager.Interact(Target);

				if (cntxt.Phase == IPhase.Post && (cntxt.Result == IResult.Cancel || cntxt.Result == IResult.Success || cntxt.Result == IResult.Fail)) End();
			}
		}
	}
}