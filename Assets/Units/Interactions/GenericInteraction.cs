using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankGame.Units.Interactions {

	public class GenericInteraction : Interaction<GenericInteraction> {
		public GenericInteraction(InteractionFunction _destination, Character _character, string _name, IInteractable _parent) : base(_destination, _character, _name, _parent) {

		}
	}

	public class GenericInteractionFactory : AbstractInteractionFactory {

		public delegate GenericInteraction ConstructionDel(Character character, string name);

		private ConstructionDel constructor;

		public GenericInteractionFactory(string _name, ConstructionDel _constructor) : base(typeof(GenericInteraction), typeof(GenericInteraction), _name) {
			constructor = _constructor;
		}

		public override Interaction Construct(Character character) {
			return constructor.Invoke(character, Name);
		}
	}
}