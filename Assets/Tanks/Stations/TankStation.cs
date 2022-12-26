using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TankGame.Units;
using TankGame.Players.Input;

namespace TankGame.Tanks.Stations {

	public class TankStation : Seat {

		public InputProcessor InputReceiver { get; private set; }

		protected Tank parentTank;

		protected virtual void Awake() {
			InputReceiver = GetComponent<InputProcessor>();
			parentTank = GetComponentInParent<Tank>();
		}
	}
}