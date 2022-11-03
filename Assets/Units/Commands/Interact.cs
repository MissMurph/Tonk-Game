using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TankGame.Units.Commands {

	public class Interact : Command<IInteractable> {

		public Interact(IInteractable target) : base(target, "interact") {
		
		}

		public override void Start(Character character) {
			base.Start(character);

			TargetTransform = Target().GetObject().transform;

			character.SubmitTarget(TargetTransform, OnPathComplete);
		}

		public override void Perform() {
			base.Perform();

			Target().Interact(Character);
			Complete();
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
		}
	}
}