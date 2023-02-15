using System;
using System.Collections;
using System.Collections.Generic;
using TankGame.Util;
using UnityEngine;

namespace TankGame.Units.Ai {

	[Serializable]
	public class HealthEval : Number {

		[SerializeField] private int value;
		[SerializeField] private Character character;

		public override bool Act () {
			//Operations can be combined as an or statement. You can have up to two Operations and as long as one Operation's condition is fulfilled, this returns true

			foreach (Operations op in Operation) {
				switch (op) {
					case Operations.LESS_THAN:
						if (character.Health < value) return true;
						break;
					case Operations.EQUAL_TO:
						if (character.Health == value) return true;
						break;
					case Operations.GREATER_THAN:
						if (character.Health > value) return true;
						break;
				}
			}

			return false;
		}

		public override string Name () {
			return "character_health";
		}

		public override double GetDouble () {
			return value;
		}

		public override float GetFloat () {
			return value;
		}

		public override int GetInt () {
			return value;
		}

		public override string GetString () {
			return value.ToString();
		}
	}
}