using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TankGame.Units;
using TankGame.Players.Input;
using TankGame.Units.Interactions;
using TankGame.Units.Ai;

namespace TankGame.Tanks.Systems.Stations {

	public class Station : System, IInteractable {

		public InputProcessor InputReceiver { get; private set; }

		protected Tank parentTank;

		public bool Manned {
			get {
				return manningCharacter != null;
			}
		}

		protected Character manningCharacter;

		protected virtual void Awake() {
			InputReceiver = GetComponent<InputProcessor>();
			parentTank = GetComponentInParent<Tank>();
			manningCharacter = null;
		}

		public GameObject GetObject() {
			return gameObject;
		}

		public List<AbstractInteractionFactory> GetInteractions() {
			return new List<AbstractInteractionFactory>() {
				new GenericInteractionFactory("test", TryTest),
			};
		}

		private InteractionContext<GenericInteraction> TestFunc(GenericInteraction interaction) {
			return new InteractionContext<GenericInteraction>(interaction, IPhase.Post, IResult.Success);
		}

		public GenericInteraction TryTest(Character character, string name) {
			return new GenericInteraction(TestFunc, character, name, this);
		}

		public List<PreRequisite> GetPreRequisites() {
			List<PreRequisite> output = new List<PreRequisite>();

			output.Add(new PreRequisite { 
				condition = new Embarkment(parentTank), 
				solution = new Interacting(parentTank.TryEmbark, "Embark") 
			});

			return output;
		}
	}
}