using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCommand : Command<Vector2> {

	public MoveCommand(Vector2 _targetPosition) : base(_targetPosition, "move") { }

	/*public override string Name() {
		return "MoveCommand";
	}*/
}