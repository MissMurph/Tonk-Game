using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TankGame.Units;
using TankGame.Players.Input;

namespace TankGame.Tanks.Stations {

	public class TankStation : MonoBehaviour {
		public bool Occupied {
			get {
				return occupant != null;
			}
		}

		protected IControllable inputReceiver;

		protected Tank parentTank;

		protected Character occupant;

		protected virtual void Awake() {
			inputReceiver = GetComponent<IControllable>();
			parentTank = GetComponentInParent<Tank>();
		}

		public IControllable GetController() {
			return inputReceiver;
		}

		public bool Embark(Character character) {
			if (Occupied) return false;

			occupant = character;

			return true;
		}

		public void Disembark() {
			if (occupant == null) return;

			occupant = null;
			parentTank.Disembark(occupant);
		}
	}
}