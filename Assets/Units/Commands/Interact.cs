using System;
using System.Collections;
using System.Collections.Generic;
using TankGame.Units.Interactions;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TankGame.Units.Commands {

	public class Interact : TargetedCommand<AbstractInteraction> {

		private InteractionManager localManager;

		public Interact(AbstractInteraction target) : base(Commands.GetTree("interact"), target) {
			
		}

		protected override void End () {
			
		}

		/*public override void Start(Character character) {
			base.Start(character);

			TargetTransform = Target.Parent.GetObject().transform;

			localManager = character.GetComponent<InteractionManager>();

			if (character.CommManager.IsInRange(TargetTransform)) {
				Perform();
				return;
			}

			character.SubmitTarget(TargetTransform, OnPathComplete);
		}

		public override void Perform() {
			if (!Character.CommManager.IsInRange(TargetTransform) || Phase.Equals(CommandPhase.Performed)) {
				return;
			}

			base.Perform();

			InteractionContext result = localManager.Interact(Target);

			if (result.Result.Equals(IResult.Success)) { Complete(); return; }
			if (result.Result.Equals(IResult.Fail) || result.Result.Equals(IResult.Cancel)) { Cancel(); return; }
		}

		public override void OnTriggerEnter(Collider2D collision) {
			base.OnTriggerEnter(collision);

			Perform();
		}

		public override void Cancel() {
			base.Cancel();

			Character.Stop();
		}

		private void OnPathComplete(bool success) {
			if (!success) Cancel();
		}*/
	}
}