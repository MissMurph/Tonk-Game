using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankGame.Units.Ai {

	public class States : MonoBehaviour {

		private delegate State StateConstructor(string name);

		private static Dictionary<string, State> registry = new Dictionary<string, State>();

		public static State IDLE_STANDING = Register("idle_standing", (name) => new IdleStanding(name));
		public static State IDLE_WANDERING = Register("idle_wandering", (name) => new IdleWandering(name));

		private static State Register (string name, StateConstructor constructor) {
			State state = constructor.Invoke(name);
			registry.TryAdd(name, state);
			return state;
		}
	}
}