using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankGame.Units.Interactions {

	public interface IInteraction<T> : IInteraction where T : IInteraction<T> {

		public delegate void InteractionFunction (T interaction);


	}

	public interface IInteraction {
		void Act ();
		IInteractable GetParent ();
	}
}