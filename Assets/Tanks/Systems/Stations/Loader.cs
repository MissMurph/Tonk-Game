using System.Collections;
using System.Collections.Generic;
using TankGame.Units;
using TankGame.Units.Interactions;
using UnityEngine;

namespace TankGame.Tanks.Systems.Stations {

	public class Loader : Station {
		public override bool Manned => throw new global::System.NotImplementedException();

		protected override Character manningCharacter => throw new global::System.NotImplementedException();

		public override GenericInteraction TryMan (Character character, string name) {
			throw new global::System.NotImplementedException();
		}
	}
}