using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankGame.Units.Ai {

	public abstract class TargetedState<T> : State {
		public override string Name { get; protected set; }
		public T Target { get; protected set; }

		public abstract override void Act (Character actor);

		public bool SetTarget (T value) {
			Target = value;
			return true;
		}
	}
}