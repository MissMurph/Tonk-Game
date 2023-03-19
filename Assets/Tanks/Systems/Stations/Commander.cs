using System;
using System.Collections;
using System.Collections.Generic;
using TankGame.Players;
using TankGame.Players.Input;
using TankGame.Units;
using TankGame.Units.Interactions;
using UnityEngine;
//using UnityEngine.InputSystem;

namespace TankGame.Tanks.Systems.Stations {

	public class Commander : Station {

		private List<Station> stations;

		private Seat localSeat;

		public override bool Manned {
            get {
                return manningCharacter is not null;
            }
        }

		protected override Character manningCharacter {
            get {
                return localSeat.Occupant;
            }
        }

		protected override void Awake() {
			base.Awake();

			localSeat = GetComponent<Seat>();

			stations = parentTank.GetStations();

			foreach (Station station in stations) {
				InputProcessor input = station.InputReceiver;
				InputProcessor receiver = InputReceiver;

				foreach (InputEntry iEntry in input.GetInputs()) {
					receiver.AddInput(iEntry);
				}
			}
		}

		protected override void Start () {
			base.Start();

			manager.AddListener<GenericInteraction>("Sit", OnSit);
			manager.AddListener<GenericInteraction>("Unsit", OnUnsit);
		}

		private void OnSit (InteractionContext<GenericInteraction> context) {
			if (ReferenceEquals(context.Interaction.ActingCharacter, Player.PlayerCharacter)) {
				Player.SwitchControl(InputReceiver);
			}
		}

		private void OnUnsit (InteractionContext<GenericInteraction> context) {
			if (ReferenceEquals(context.Interaction.ActingCharacter, Player.PlayerCharacter)) {
				Player.ResetControl();
			}
		}

		public override GenericInteraction TryMan(Character character, string name) {
			return localSeat.TrySit(character, "Sit");
		}
	}
}