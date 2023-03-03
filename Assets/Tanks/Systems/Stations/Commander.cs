using System;
using System.Collections;
using System.Collections.Generic;
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

			stations = parentTank.GetStations();

			foreach (Station station in stations) {
				InputProcessor input = station.InputReceiver;
				InputProcessor receiver = InputReceiver;

				foreach (InputEntry iEntry in input.GetInputs()) {
					receiver.AddInput(iEntry);
				}
			}
		}

		public override GenericInteraction TryMan(Character character, string name) {
			return localSeat.TrySit(character, "Sit");
		}
	}
}