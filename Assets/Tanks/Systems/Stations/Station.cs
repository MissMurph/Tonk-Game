using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TankGame.Units;
using TankGame.Players.Input;
using TankGame.Units.Interactions;
using TankGame.Units.Ai;

namespace TankGame.Tanks.Systems.Stations {

	public abstract class Station : System, IInteractable {

		public InputProcessor InputReceiver { get; private set; }

		protected Tank parentTank;

		public abstract bool Manned { get; }

		protected abstract Character manningCharacter { get; }

		protected override void Awake() {
			base.Awake();
			InputReceiver = GetComponent<InputProcessor>();
			parentTank = GetComponentInParent<Tank>();
		}

		public override List<AbstractInteractionFactory> GetInteractions() {
			List<AbstractInteractionFactory> output = base.GetInteractions();

			output.Add(new GenericInteractionFactory("man_station", TryMan));

			return output;
		}

		public abstract GenericInteraction TryMan(Character character, string name); /*{
			return !Manned ? new GenericInteraction(ManStation, character, name, this) : null;
		}*/

		public List<PreRequisite> GetPreRequisites() {
			List<PreRequisite> output = new List<PreRequisite>();

			output.Add(new PreRequisite { 
				condition = new NeedsToEmbark(parentTank), 
				solution = new Interacting(parentTank.TryEmbark, "Embark") 
			});

			return output;
		}
	}
}