using System;
using System.Collections;
using System.Collections.Generic;
using TankGame.Players.Input;
using UnityEngine;
//using UnityEngine.InputSystem;

namespace TankGame.Tanks.Systems.Stations {

	public class Commander : Station {

		List<Station> stations;

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
	}
}