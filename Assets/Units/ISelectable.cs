using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankGame.Units.Commands {

	public interface ISelectable {
		ISelectable Select();
		GameObject GetObject();
		void EnqueueCommand(Command command);
		void ExecuteCommand(Command command);
	}
}