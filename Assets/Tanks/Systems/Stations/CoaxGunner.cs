using TankGame.Units;
using TankGame.Units.Interactions;
using UnityEngine;


namespace TankGame.Tanks.Systems.Stations {

	public class CoaxGunner : Station {
		public override bool Manned { get { return true; } }

		protected override Character manningCharacter => throw new global::System.NotImplementedException();

		public override GenericInteraction TryMan(Character character, string name) {
			throw new global::System.NotImplementedException();
		}

		void Start() {

		}

		void Update() {

		}
	}
}