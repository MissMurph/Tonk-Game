using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TankGame.Units;
using TankGame.Players.Input;

namespace TankGame.Tanks.Systems.Stations {

	public class Station : System {

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
	}
}