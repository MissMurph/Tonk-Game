using System;
using System.Collections;
using System.Collections.Generic;
using TankGame.Players.Input;
using UnityEngine;
//using UnityEngine.InputSystem;

namespace TankGame.Tanks.Stations {

	public class CommandStation : TankStation {

		List<TankStation> stations;

		protected override void Awake() {
			base.Awake();

			stations = parentTank.GetStations();

			foreach (TankStation station in stations) {
				InputProcessor input = (InputProcessor)station.GetController();
				InputProcessor receiver = (InputProcessor)GetController();

				foreach (InputEntry iEntry in input.GetInputs()) {
					receiver.AddInput(iEntry);
				}
			}
		}
	}
}