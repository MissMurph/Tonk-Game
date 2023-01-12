using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankGame.Units.Ai {

	public class IdleStanding : State {
		public override string Name { get;  protected set; }
		public override Transform Target { get; protected set; }

		public IdleStanding (string _name) {
			Name = _name;
		}

		public IdleStanding () {

		}

		public override void Act (Character actor) {
			//Target = actor.transform;
		}
	}
}