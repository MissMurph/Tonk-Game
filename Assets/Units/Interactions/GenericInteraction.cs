using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankGame.Units.Interactions {

	public class GenericInteraction : IInteraction<GenericInteraction> {

		private IInteraction<GenericInteraction>.InteractionFunction yeet;

		public void Act () {
			yeet.Invoke(this);
		}

		public IInteractable GetParent () {
			throw new System.NotImplementedException();
		}
	}
}